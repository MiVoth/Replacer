using System;
using System.Collections;
using System.Text;
using MiVo.Text.Replacer.Interfaces;

namespace MiVo.Text.Replacer.Entities
{
    public class Replacement : IReplacement
    {
        public ReplacementType Type { get; set; }
        public object Value { get; set; } = null!;

        public override string ToString()
        {
            if (Type == ReplacementType.Template)
            {
                StringBuilder sb = new($"{Value} ({Type})");
                try
                {
                    IEnumerable? liste = Value as IEnumerable;
                    if (liste != null)
                    {

                        int i = 1;
                        foreach (object item in liste)
                        {
                            sb.AppendLine($"---- [{i}] {item}");
                            i++;
                        }
                    }
                }
                catch (Exception) { }
                return sb.ToString();
            }
            else
            {
                return $"{Value} ({Type})";
            }
        }
    }
}