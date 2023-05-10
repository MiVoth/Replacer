using MiVo.Text.Replacer.Interfaces;

namespace MiVo.Text.Replacer
{
    public static class ReplacerFactory
    {
        public static IReplacer GetReplacer(string input, ReplacerConfig? config = null)
        {
            if (config == null)
            {
                config = new();
            }
            IReplacer replacer = new Replacer(input, config);
            // {
            //     ReplacePattern = config.ReplacePattern,
            //     RemovePatternStart = config.RemovePatternStart,
            //     RemovePatternEnd = config.RemovePatternEnd,
            //     ReplaceDataName = config.ReplaceDataName,
            //     ShowDataName = config.ShowDataName,
            //     HideDataName = config.HideDataName
            // };
            return replacer;
        }
    }
}