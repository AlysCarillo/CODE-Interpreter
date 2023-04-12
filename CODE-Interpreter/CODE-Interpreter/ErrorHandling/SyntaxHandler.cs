using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CODE_Interpreter.Content;

namespace CODE_Interpreter.ErrorHandling
{
    internal class SyntaxHandler : BaseErrorListener
    {
        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            string errorType = "Syntax Error";
            Console.WriteLine($"{errorType} at line {line}: {msg}");
            Environment.Exit(400);
        }
    }
}