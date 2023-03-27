using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using CODE_Interpreter.Content;
using CODE_Interpreter;

var file = "test.code";
var fileContents = File.ReadAllText(file);

var inputStream = new AntlrInputStream(fileContents);

// Create a lexer and parser for the code
var codeLexer = new CodeLexer(inputStream);
CommonTokenStream commonTokenStream = new CommonTokenStream(codeLexer);
var codeParser = new CodeParser(commonTokenStream);
var codeContext = codeParser.program();

// Parse the code and walk the parse tree using the CodeVisitor
var visitor = new CodeVisitor();
visitor.VisitProgram(codeContext);