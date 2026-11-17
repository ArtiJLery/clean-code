using Markdown;

var input = "# Заголовок с разными символами __ и _курсивом___";
var output = new Md().Render(input);
Console.WriteLine(output);