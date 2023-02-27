using System;
using System.Reflection;
using MiVo.Text.Replacer.Interfaces;

namespace MiVo.Text.Replacer.Entities
{
    public class Replacement : IReplacement
    {
        public ReplacementType Type { get; set; }
        public object Value { get; set; } = null!;
    }
}