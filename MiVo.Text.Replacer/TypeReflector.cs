using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MiVo.Text.Replacer.Entities;

namespace MiVo.Text.Replacer
{
    static class TypeReflector
    {
        public static PropertyInfo[] GetPublicProperties(this Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface)) { continue; }

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(
                        BindingFlags.FlattenHierarchy
                        | BindingFlags.Public
                        | BindingFlags.Instance);

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy
                | BindingFlags.Public | BindingFlags.Instance);
        }
        public static void Reflector(object bndl, Type t, Replacer rpl, bool withPrefix = false, string? nameBefore = null)
        {
            var bndlprops = t.GetPublicProperties();
            foreach (var prop in bndlprops)
            {
                string propName = prop.Name;
                if (withPrefix && !string.IsNullOrEmpty(nameBefore))
                {
                    propName = $"{nameBefore}_{prop.Name}";
                }
                if (prop.PropertyType == typeof(string)
                    || prop.PropertyType == typeof(long)
                    || prop.PropertyType == typeof(long?)
                    || prop.PropertyType == typeof(int)
                    || prop.PropertyType == typeof(int?)
                    || prop.PropertyType == typeof(short)
                    || prop.PropertyType == typeof(short?)
                    || prop.PropertyType == typeof(decimal)
                    || prop.PropertyType == typeof(decimal?))
                {
                    string sval = "";
                    if (bndl != null)
                    {
                        sval = $"{prop.GetValue(bndl)}";
                    }
                    rpl.AddStringReplacement(propName, sval);
                    rpl.AddRemoveRelation(propName, !string.IsNullOrEmpty(sval));
                }
                else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
                {
                    bool sval = false;
                    if (bndl != null)
                    {
                        var val = prop.GetValue(bndl);
                        if (val != null)
                        {
                            sval = (bool)val;
                        }
                    }
                    rpl.AddRemoveRelation(propName, sval);
                }
                else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                {
                    var sval = "";
                    if (bndl != null)
                    {
                        var val = prop.GetValue(bndl);
                        if (val != null)
                        {
                            DateTime tm = ((DateTime)val);
                            if (tm.Second == 0 && tm.Minute == 0 && tm.Hour == 0)
                            {
                                sval = tm.ToShortDateString();
                            }
                            else
                            {
                                sval = tm.ToString();
                            }
                        }
                    }
                    if (prop.PropertyType == typeof(DateTime?))
                    {
                        rpl.AddRemoveRelation(propName, !string.IsNullOrEmpty(sval));
                    }
                    rpl.AddStringReplacement(propName, sval);
                }
                else if (prop.PropertyType.IsInterface || prop.PropertyType.IsClass)
                {
                    // if (bndl != null)
                    // {
                    //     object? obj2 = prop.GetValue(bndl);
                    //     if (obj2 != null)
                    //     {
                    //         if (prop.PropertyType.IsGenericType &&
                    //         (prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>) ||
                    //         prop.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                    //         {
                    //             rpl.ReplacementRelation.Add(propName, new Replacement
                    //             {
                    //                 Type = ReplacementType.Template,
                    //                 Value = obj2
                    //             });
                    //         }
                    //         else
                    //         {
                    //             Reflector(obj2, prop.PropertyType, rpl, withPrefix, propName);
                    //         }
                    //     }
                    // }
                    if (prop.PropertyType.IsGenericType &&
                    (prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>) ||
                    prop.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                    {
                        object? val = bndl == null ? null : prop.GetValue(bndl);
                        if (val != null)
                        {
                            object? obj2 = prop.GetValue(bndl);
                            if (obj2 != null)
                            {
                                rpl.ReplacementRelation.Add(propName, new Replacement
                                {
                                    Type = ReplacementType.Template,
                                    Value = obj2
                                });
                            }
                        }
                    }
                    else
                    {
                        if (bndl != null)
                        {
                            if (propName.StartsWith("_"))
                            {
                                propName = string.Empty;
                            }
                            object? objValue = prop.GetValue(bndl);
                            if (withPrefix && !string.IsNullOrEmpty(propName))
                            {
                                rpl.RemoveRelation.Add(propName, objValue != null);

                            }
                            if (objValue != null)
                            {
                                Reflector(objValue, prop.PropertyType, rpl, withPrefix, propName);
                            }
                            else if (!string.IsNullOrEmpty(propName))
                            {
                                rpl.RemoveRelation.Add(propName, false);
                            }
                        }
                        else
                        {
                            rpl.RemoveRelation.Add(propName, false);
                        }
                    }
                }
                else
                {

                }
            }
        }
    }
}