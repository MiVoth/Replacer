using System;
using System.Collections.Generic;
using System.Linq;

namespace MiVo.Text.Replacer.Tests;

public class PocoClass : PocoAbstractClass
{
  public string Title { get; set; } = string.Empty;
  public string AddSomething { get; set; } = string.Empty;
  public bool WithRegards { get; set; } = false;
  public string Style { get; set; } = string.Empty;
}
public class PocoClass2 : PocoAbstractClass
{
  public string Description { get; set; } = string.Empty;
}

public abstract class PocoAbstractClass
{
  public List<string> ExampleList { get; set; } = [];
}