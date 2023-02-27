using MiVo.Text.Replacer.Entities;

namespace MiVo.Text.Replacer.Interfaces
{
    public interface IReplacement
    {
        ReplacementType Type { get; set; }
        object Value { get; set; }

    }
}