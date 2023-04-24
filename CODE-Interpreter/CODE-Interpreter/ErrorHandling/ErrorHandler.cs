using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CODE_Interpreter.ErrorHandling
{
    public class ErrorHandler
    {
        public static bool HandleTypeError([NotNull] ParserRuleContext context, object? obj, Type? type, string location)
        {
            if (obj is int || obj is float || obj is bool || obj is char || obj is string)
            {
                if (obj.GetType() == type)
                {
                    return true;
                }
                else
                {
                    var line = context.Start.Line;
                    Console.WriteLine($"Semantic Error: in {location}, line {line}.");
                    Console.WriteLine($"Cannot convert {obj?.GetType().Name.ToUpper()} to {type?.Name.ToUpper()}.");
                    Environment.Exit(400);
                    return false;
                }
            }
            else
            {
                var line = context.Start.Line;
                Console.WriteLine($"Semantic Error: in {location}, line {line}.");
                Console.WriteLine($"Cannot convert {obj?.GetType().Name.ToUpper()} to {type?.Name.ToUpper()}.");
                Environment.Exit(400);
                return false;
            }
        }
    }
}
