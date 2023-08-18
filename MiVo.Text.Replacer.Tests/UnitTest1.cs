using MiVo.Text.Replacer.Interfaces;

namespace MiVo.Text.Replacer.Tests;
public class UnitTest1
{
  [Fact]
  public void Test1()
  {
    Assert.Equal(true, MiVo.Text.Replacer.BasicClass.TestIt());
  }

  [Fact]
  public void Test2()
  {
    string input = "<div>Hello</div><div data-replace=\"hello\"></div>";
    IReplacer replacer = ReplacerFactory.GetReplacer(input, new ReplacerConfig
    {
    });

    string output = replacer.GetText(new
    {
      hello = "world"
    });
    Assert.Equal(true, output.Contains("world"));
  }

  [Fact]
  public void ExampleTest2()
  {
    string input = @"
        <html> 
  <head></head>
  <body> 
    <h1 data-replace=""Title""></h1>
    <hr />
    <p>
      Start of the text, <br>
      but i want to add [*AddSomething*]
    </p>
    <hr />
    <div data-remove=""WithRegards"">
      With regards
    </div>
  </body>
</html>
";
    IReplacer replacer = ReplacerFactory.GetReplacer(input);

    string output = replacer.GetText(new
    {
      Title = "My Title",
      AddSomething = "something to add",
      WithRegards = false
    });
    Assert.Equal(true, output.Contains("something to"));
  }

  [Fact]
  public void ReplaceStyleTest()
  {
    string input = @"
        <html> 
  <head><style data-replace=""style""></style></head>
  <body> 
    <h1 data-replace=""Title""></h1>
    <hr />
    <p>
      Start of the text, <br>
      but i want to add [*AddSomething*]
    </p>
    <hr />
    <div data-remove=""WithRegards"">
      With regards
    </div>
  </body>
</html>
";
    IReplacer replacer = ReplacerFactory.GetReplacer(input);

    string output = replacer.GetText(new
    {
      Title = "My Title",
      AddSomething = "something to add",
      WithRegards = false,
      style = "body: color:black;"
    });
    System.IO.File.WriteAllText(@"C:\workspace\rplx.html", output);
    Assert.Equal(true, output.Contains("something to"));
    Assert.Equal(true, output.Contains("body: color:black;"));
  }
}