using System.Text;

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
        public TypeReflectorConfig TypeReflectorConfig { get; set; } = new();
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"ReplacePattern: {ReplacePattern}");
            sb.AppendLine($"RemovePattern: {RemovePatternStart} - {RemovePatternEnd}");
            sb.AppendLine($"ReplaceDataName: {ReplaceDataName}");
            sb.AppendLine($"ShowDataName: {ShowDataName}");
            sb.AppendLine($"HideDataName: {HideDataName}");
            sb.AppendLine($"TypeReflectorConfig: {TypeReflectorConfig}");
            return sb.ToString();
            // return base.ToString();
        }
    }

    public class TypeReflectorConfig
    {
        public bool WithPrefix { get; set; } = false;
        public bool AutoInsertBooleanText { get; set; } = false;
        public string AutoBooleanTextTrue { get; set; } = "True";
        public string AutoBooleanTextFalse { get; set; } = "False";
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"WithPrefix: {WithPrefix}");
            sb.AppendLine($"AutoInsertBooleanText: {AutoInsertBooleanText}");
            sb.AppendLine($"AutoBooleanTextTrue: {AutoBooleanTextTrue}");
            sb.AppendLine($"AutoBooleanTextFalse: {AutoBooleanTextFalse}");
            return sb.ToString();
            // return base.ToString();
        }
    }
}