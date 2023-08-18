using System.ComponentModel.DataAnnotations;
using MiVo.Text.Replacer.Interfaces;

namespace MiVo.Text.Replacer.Tests;

public class EnumTests
{
    private enum TestEnum
    {
        Enum123 = 1,
        [Display(Name = "DisplayEnum456")] Enum456 = 2
    }
    private class EnumTestClass
    {
        public TestEnum EnumProperty { get; set; }
    }
    [Fact]
    public void EnumToString()
    {
        string input = "<div data-replace=\"EnumProperty\"></div>";
        IReplacer replacer = ReplacerFactory.GetReplacer(input, new ReplacerConfig
        {
        });

        string output = replacer.GetText(new EnumTestClass
        {
            EnumProperty = TestEnum.Enum123
        });
        Assert.Contains($"{TestEnum.Enum123}", output);
    }

    [Fact]
    public void EnumDisplayToString()
    {
        string input = "<div data-replace=\"EnumProperty\"></div>";
        IReplacer replacer = ReplacerFactory.GetReplacer(input, new ReplacerConfig
        {
        });

        string output = replacer.GetText(new EnumTestClass
        {
            EnumProperty = TestEnum.Enum456
        });
        Assert.Contains($"DisplayEnum456", output);
    }
}
