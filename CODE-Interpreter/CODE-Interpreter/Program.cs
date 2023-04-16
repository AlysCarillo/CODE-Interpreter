using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using CODE_Interpreter.Content;
using CODE_Interpreter;
using CODE_Interpreter.ErrorHandling;
using System;

bool isContinue = true;

while (isContinue)
{
    var file = "..\\..\\..\\Content\\tests.txt";
    var fileContents = File.ReadAllText(file);

    var inputStream = new AntlrInputStream(fileContents);

    // Create a lexer and parser for the code
    var lexer = new CodeLexer(inputStream);
    var tokens = new CommonTokenStream(lexer);
    var parser = new CodeParser(tokens);

    // Error Handling
    var syntaxHandler = new SyntaxHandler();
    parser.AddErrorListener(syntaxHandler);

    var codeContext = parser.program();

    // Parse the code and walk the parse tree using the CodeVisitor
    var visitor = new CodeVisitor();
    visitor.VisitProgram(codeContext);

    Console.WriteLine("\n");
    Console.WriteLine("Continue? (Y/N): ");
    var res = Console.ReadLine()![0];

    isContinue = (res != 'Y' || res != 'y') ? false : true;

    Console.WriteLine("=================================================================================");
    Console.WriteLine("\n");

}



