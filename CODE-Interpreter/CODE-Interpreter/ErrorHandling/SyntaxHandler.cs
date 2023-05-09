using Antlr4.Runtime;

namespace CODE_Interpreter.Methods
{
    public class ErrorHandling : BaseErrorListener
    {
        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            string errorType = "Syntax Error";
            Console.WriteLine($"{errorType} at line {line}: {msg}");
            Environment.Exit(400);
        }

    }
}