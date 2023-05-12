# Replacer - a text replacer based on HtmlAgilityPack

A simple library to replace text-snippets in HTML files or simple text files.

## Example
```html
<!-- input string -->
<html>
  <head></head>
  <body>
    <h1 data-replace="Title"></h1>
    <hr />
    <p>
      Start of the text, <br>
      but i want to add [*AddSomething*]
    </p>
    <hr />
    <div data-remove="WithRegards">
      With regards
    </div>
  </body>
</html>
```

Using Replacer like that:
```csharp
string input = System.IO.File.ReadAllText(...);
IReplacer replacer = ReplacerFactory.GetReplacer(input);

string output = replacer.GetText(new
{
    Title = "My Title",
    AddSomething = "something to add",
    WithRegards = false
});
```

Results in
```html
<!-- output string -->
<html> 
  <head></head>
  <body> 
    <h1 data-replace="Title" data-done="true">My Title</h1>
    <hr>
    <p>
      Start of the text, <br>
      but i want to add something to add
    </p>
    <hr>
    <div data-remove="WithRegards">
      With regards
    </div>
  </body>
</html>
```

Extended documentation is following.
