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
    var codeLexer = new CodeLexer(inputStream);
    CommonTokenStream commonTokenStream = new CommonTokenStream(codeLexer);
    var codeParser = new CodeParser(commonTokenStream);

    // Error Handling
    var syntaxHandler = new SyntaxHandler();
    codeParser.AddErrorListener(syntaxHandler);

    var codeContext = codeParser.program();

    // Parse the code and walk the parse tree using the CodeVisitor
    var visitor = new CodeVisitor();
    visitor.VisitProgram(codeContext);

    Console.WriteLine("\n");
    Console.WriteLine("Finish? (Y/N): ");
    var res = Console.ReadLine()![0];

    isContinue = (res == 'N' || res == 'n') ? true : false;

    Console.WriteLine("=================================================================================");
    Console.WriteLine("\n\n");

}



