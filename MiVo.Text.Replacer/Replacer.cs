using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HtmlAgilityPack;
using MiVo.Text.Replacer.Entities;
using MiVo.Text.Replacer.Interfaces;

namespace MiVo.Text.Replacer
{
    public class ReplacerConfig
    {
        public string ReplacePattern { get; set; } = "[*{0}*]";
        public string RemovePatternStart { get; set; } = "[?{0}?]";
        public string RemovePatternEnd { get; set; } = "[?/{0}?]";
        public string ReplaceDataName { get; set; } = "data-replace";
        public string ShowDataName { get; set; } = "data-show"; //"data-remove";
        public string HideDataName { get; set; } = "data-hide"; //"data-remove";
        public bool TypeReflectorWithPrefix { get; set; } = false;
    }

    class Replacer : IReplacer
    {
        private bool Blue { get; set; } = false;
        private readonly string _input;
        public string ReplacePattern { get => Config.ReplacePattern; }
        public string RemovePatternStart { get => Config.RemovePatternStart; }
        public string RemovePatternEnd { get => Config.RemovePatternEnd; }
        public string ReplaceDataName { get => Config.ReplaceDataName; }
        public string ShowDataName { get => Config.ShowDataName; }
        public string HideDataName { get => Config.HideDataName; }

        public List<Exception> Errors { get; set; }
        public ReplacerConfig Config { get; }

        public Replacer(string input, ReplacerConfig config)
        {
            _input = input;
            Errors = new();
            Config = config;
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, IReplacement> ReplacementRelation { get; set; } = new Dictionary<string, IReplacement>();

        /// <summary>
        /// key == name to look for
        /// value == determines if block should be shown
        /// </summary>
        public Dictionary<string, bool> RemoveRelation { get; set; } = new Dictionary<string, bool>();

        public string GetText<T>(T obj)
        {
            return GetText(obj, typeof(T));
        }
        public string GetText(object? obj = null, Type? t = null)
        {
            if (obj != null && t != null)
            {
                TypeReflector.Reflector(obj, t, this, Config.TypeReflectorWithPrefix, null);
            }
            string txt = _input;
            txt = SimpleTextReplaceAndRemove(txt);
            txt = AgilityReplaceAndRemove(txt);

            foreach (string key in RemoveRelation.Keys)
            {
                txt = CheckAndReplaceIf(key, RemoveRelation[key], txt);
            }
            if (Blue)
            {
                string css = ".green-highlight, span[data-remove], div[data-remove],span[data-remove-not], div[data-remove-not] { /*color: darkgreen;*/ } .blue-highlight, *[data-replace], *[data-replace][data-remove] { /*color: blue;*/ }";
                if (txt.Contains("<style>"))
                {
                    txt = txt.Replace("<style>", $"<style> {css}");
                }
                else
                {
                    txt = txt.Replace("<style type=\"text/css\">", $"<style type=\"text/css\"> {css}");
                }
            }
            return txt;
        }

        private string SimpleTextReplaceAndRemove(string txt)
        {
            StringBuilder sb = new StringBuilder(_input);
            foreach (var key in ReplacementRelation.Keys)
            {
                switch (ReplacementRelation[key].Type)
                {
                    case ReplacementType.Html:
                    case ReplacementType.String:
                        string rr = $"{ReplacementRelation[key].Value}";
                        if (string.IsNullOrEmpty(rr))
                        {
                            rr = "&nbsp;";
                        }
                        if (Blue)
                        {
                            rr = $"<span class=\"blue-highlight\">{rr}</span>";
                        }
                        sb.Replace(string.Format(ReplacePattern, key), rr);
                        break;
                    case ReplacementType.List:
                        var liste = (List<string>)ReplacementRelation[key].Value;
                        string bluecss = Blue ? " class=\"blue-highlight\" " : "";
                        StringBuilder sb2 = new StringBuilder($"<ul {bluecss}>");
                        foreach (var item in liste)
                        {
                            sb2.Append($"<li>{item}</li>");
                        }
                        sb2.Append("</ul>");
                        sb.Replace(string.Format(ReplacePattern, key), sb2.ToString());
                        break;
                    case ReplacementType.Image:

                        if (ReplacementRelation[key].Value.GetType() == typeof(byte[]))
                        {
                            var by = (byte[])ReplacementRelation[key].Value;
                            var str = Convert.ToBase64String(by);
                            sb.Replace(string.Format(ReplacePattern, key), str);
                        }
                        else
                        {
                            sb.Replace(string.Format(ReplacePattern, key), (string)ReplacementRelation[key].Value);
                        }
                        break;
                    default:
                        break;
                }
            }
            return sb.ToString();
        }

        private string GetReplacement(ReplacementType rtype, object value1)
        {
            string result = "";
            switch (rtype)
            {
                case ReplacementType.String:
                    string rr = (string)value1;
                    if (string.IsNullOrEmpty(rr))
                    {
                        rr = "";
                    }
                    result = rr;
                    break;
                case ReplacementType.List:
                    List<string> liste = (List<string>)value1;
                    StringBuilder sb2 = new StringBuilder($"<ul>");
                    foreach (string item in liste)
                    {
                        sb2.Append($"<li>{item}</li>");
                    }
                    sb2.Append("</ul>");
                    result = sb2.ToString();
                    break;
                case ReplacementType.Image:
                    if (value1.GetType() == typeof(byte[]))
                    {
                        byte[] by = (byte[])value1;
                        string str = Convert.ToBase64String(by);
                        result = str;
                    }
                    else
                    {
                        result = (string)value1;
                    }
                    break;
                case ReplacementType.Html:
                    string value = (string)value1;
                    value = value ?? "";
                    result = value;
                    //node.InnerHtml = value;
                    //node.Attributes.RemoveAll();
                    break;
                    //case ReplacementType.Docx:
                    //    break;
                    // case ReplacementType.Template:
                    //     var tbl = (System.Collections.IEnumerable)value1;
                    //     var tabelle = GetTabelle(tbl);
                    //     result = tabelle;
                    //node.InnerHtml = tabelle;
                    // break;
                    //default:
                    //    break;
            }
            return result;
        }


        // uses Agility-Package so look for data-attributes with the key-names
        private string AgilityReplaceAndRemove(string txt)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(txt);

            foreach (var key in ReplacementRelation.Keys)
            {
                HtmlNodeCollection? replaceNodes = doc.DocumentNode.SelectNodes($"//*[@{ReplaceDataName}='{key}']");
                if (replaceNodes != null)
                {
                    foreach (var node in replaceNodes)
                    {
                        // if tag was already processed it will be ignored
                        bool done = node.GetAttributeValue("data-done", "") == "true";
                        if (!done)
                        {
                            if (ReplacementRelation[key].Type == ReplacementType.Html)
                            {
                                node.Attributes.RemoveAll();
                            }

                            if (ReplacementRelation[key].Type == ReplacementType.Template)
                            {
                                try
                                {
                                    IEnumerable liste = (IEnumerable)ReplacementRelation[key].Value;
                                    StringBuilder sb = new("");

                                    foreach (object item in liste)
                                    {
                                        HtmlNode clone = node.CloneNode(true);
                                        string r = clone.InnerHtml;
                                        Replacer rpl = new(r, Config);
                                        Type itemType = item.GetType();
                                        clone.InnerHtml = rpl.GetText(item, itemType);
                                        node.ParentNode.InsertBefore(clone, node);
                                    }
                                    node.Remove();
                                }
                                catch (Exception) { }
                            }
                            else
                            {
                                node.InnerHtml = GetReplacement(ReplacementRelation[key].Type, ReplacementRelation[key].Value);
                            }
                        }
                        node.SetAttributeValue("data-done", "true");
                    }
                }
            }

            foreach (var key in RemoveRelation.Keys)
            {
                HtmlNodeCollection? removeNodes = doc.DocumentNode.SelectNodes($"//*[@{ShowDataName}='{key}']");
                if (removeNodes != null)
                {
                    if (!RemoveRelation[key])
                    {
                        foreach (HtmlNode? node in removeNodes)
                        {
                            node.ParentNode.RemoveChild(node);
                        }
                    }
                }
                // remove-not
                removeNodes = doc.DocumentNode.SelectNodes($"//*[@{HideDataName}='{key}']");
                if (removeNodes != null)
                {
                    if (RemoveRelation[key])
                    {
                        foreach (HtmlNode? node in removeNodes)
                        {
                            node.ParentNode.RemoveChild(node);
                        }
                    }
                }

            }
            using (var sw = new StringWriter())
            {
                doc.Save(sw);
                return sw.ToString();
            }
        }

        private string CheckAndReplaceIf(string tagname, bool? val, string html)
        {
            string result = "";
            bool yes = val ?? false;
            string starttag = string.Format(RemovePatternStart, tagname); // "[?" + tagname + "?]";
            string endtag = string.Format(RemovePatternEnd, tagname); // "[?/" + tagname + "?]";
            if (!yes)
            {
                int first = html.IndexOf(starttag);
                int last = html.IndexOf(endtag);
                if (first > -1 && last > -1)
                {
                    last += endtag.Length;
                    result = html.Substring(0, first) + html.Substring(last, html.Length - last);
                }
                else
                {
                    result = html.Replace(starttag, "");
                    result = result.Replace(endtag, "");
                }
            }
            else
            {
                string begin = "";
                string end = "";
                if (Blue)
                {
                    begin = "<span class=\"green-highlight\">";
                    end = "</span>";
                }
                result = html.Replace(starttag, begin);
                result = result.Replace(endtag, end);
            }
            return result;
        }
        public void AddString(string pattern, string replacement)
        {
            AddStringReplacement(pattern, replacement);
            AddRemoveRelation(pattern, !string.IsNullOrEmpty(replacement));
        }

        public void AddStringReplacement(string pattern, string replacement)
        {
            if (ReplacementRelation.ContainsKey(pattern))
            {
                ReplacementRelation[pattern] = new Replacement { Type = ReplacementType.String, Value = replacement };
            }
            else
            {
                ReplacementRelation.Add(pattern, new Replacement { Type = ReplacementType.String, Value = replacement });
            }
        }

        public void AddImageReplacement(string pattern, string replacement)
        {
            ReplacementRelation.Add(pattern, new Replacement { Type = ReplacementType.Image, Value = replacement });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern">search pattern</param>
        /// <param name="val">true => show; false => remove everything </param>
        public void AddRemoveRelation(string pattern, bool val)
        {
            if (RemoveRelation.ContainsKey(pattern))
            {
                RemoveRelation[pattern] = val;
            }
            else
            {
                RemoveRelation.Add(pattern, val);
            }
        }

        public void AddReplacement(string pattern, IReplacement replacement)
        {
            if (ReplacementRelation.ContainsKey(pattern))
            {
                ReplacementRelation[pattern] = replacement;
            }
            else
            {
                ReplacementRelation.Add(pattern, replacement);
            }
        }
    }
}
