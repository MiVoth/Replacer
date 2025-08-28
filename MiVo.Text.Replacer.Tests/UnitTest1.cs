using System;
using System.Collections.Generic;
using System.Linq;
using MiVo.Text.Replacer.Interfaces;

namespace MiVo.Text.Replacer.Tests;

public class UnitTest1
{
  [Fact]
  public void Test1()
  {
    Assert.True(MiVo.Text.Replacer.BasicClass.TestIt());
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
    Assert.Contains("world", output);
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
    Assert.Contains("something to", output);
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
    Assert.Contains("something to", output);
    Assert.Contains("body: color:black;", output);
  }

  [Fact]
  public void TestList()
  {
    string input = "<html><body><p data-replace=\"List\"><span data-replace=\"AddSomething\"></span></p></body></html>";
    IReplacer replacer = ReplacerFactory.GetReplacer(input);
    List<PocoClass> list = new List<PocoClass>
    {
      new PocoClass { Title = "Title 1", AddSomething = "Add 1", WithRegards = true, Style = "color:red;" },
      new PocoClass { Title = "Title 2", AddSomething = "Add 2", WithRegards = false, Style = "color:blue;" }
    };
    List<PocoClass> emptyList = [];
    string output = replacer.GetText(new
    {
      List = list,
      List2 = emptyList.AsEnumerable()
    });
    // string text = "this ist text";
    string output2 = replacer.GetText(new
    {
      Poco1 = new PocoClass()
      {
        ExampleList = ["One", "Two"]
      },
      Poco2 = new PocoClass2
      { 
        ExampleList = ["Three", "Four"]
      }
    });
    Assert.Contains("Add 1", output);
    Assert.Contains("Add 2", output);
    Assert.Contains("List", output2);
  }
}
