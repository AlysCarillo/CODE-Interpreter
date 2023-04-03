using Antlr4.Runtime.Misc;
using CODE_Interpreter.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using static Antlr4.Runtime.Atn.SemanticContext;

public class CodeVisitor : CodeBaseVisitor<object>
{
    private Dictionary<string, object> Variables = new Dictionary<string, object>();

    public CodeVisitor()
    {
        //Variables["DISPLAY:"] = new Func<object?[], object?>();
        //Variable["SCAN:"] = new Func<object?[], object?>(Scan);
    }

    public override object VisitProgram([NotNull] CodeParser.ProgramContext context)
    {
        string code = context.GetText().Trim();
        try
        {
            if (code.StartsWith("BEGIN CODE") && code.EndsWith("END CODE"))
                Console.WriteLine("Success");
        }
        catch
        {
            Console.WriteLine("Code must start with 'BEGIN CODE' and end with 'END CODE'.");
            throw new ArgumentException("Code must start with 'BEGIN CODE' and end with 'END CODE'.");
        }

        // Visit all variable declarations first
        foreach (var variableDeclaration in context.variableDeclaration())
        {
            Visit(variableDeclaration);
        }

        // Visit all statements next
        foreach (var statement in context.statement())
        {
            VisitStatement(statement);
        }
        return new object();
    }

    public override object VisitStatement([NotNull] CodeParser.StatementContext context)
    {
        if(context.assignmentStatement() != null)
        {
            return VisitAssignmentStatement(context.assignmentStatement());
        }
        else if(context.displayStatement() != null)
        {
            return VisitDisplayStatement(context.displayStatement());
        }
        else if(context.scanStatement() != null)
        {
            return VisitScanStatement(context.scanStatement());
        }
        else
        {
            return new object();
        }
    }

    public override object VisitVariableDeclaration([NotNull] CodeParser.VariableDeclarationContext context)
    {
        var type = context.dataType().GetText();
        var varName = context.variableList().Select(x => x.GetText()).ToArray();

        var varValue = VisitExpression(context.expression());

        foreach (var vars in varName)
        {
            if (Variables.ContainsKey(vars))
            {
                Console.WriteLine($"Variable '{vars}' is already defined!");
            }
            else
            {
                if (type.Equals("INT"))
                {
                    if (int.TryParse(varValue.ToString(), out int intValue))
                    {
                        Variables[vars] = intValue;
                    }
                    else
                    {
                        int value;
                        bool success = int.TryParse(varValue.ToString(), out value);
                        if (!success)
                        {
                            Console.WriteLine($"Invalid value for integer variable '{vars}'");
                        }
                    }
                }
                else if (type.Equals("FLOAT"))
                {
                    if (float.TryParse(varValue.ToString(), out float floatValue))
                        return Variables[vars] = floatValue;
                    else
                        Console.WriteLine($"Invalid value for float variable '{vars}'");
                }
                else if (type.Equals("BOOL"))
                {
                    if (bool.TryParse(varValue.ToString(), out bool boolValue))
                        return Variables[vars] = boolValue;
                    else
                        Console.WriteLine($"Invalid value for boolean variable '{vars}'");
                }
                else if (type.Equals("CHAR"))
                {
                    var charValue = varValue.ToString();
                    if (charValue?.Length == 3 && charValue[0] == '\'' && charValue[2] == '\'')
                        return Variables[vars] = charValue[1];
                    else
                        Console.WriteLine($"Invalid value for character variable '{vars}'");
                }
                else
                {
                    Console.WriteLine($"Invalid variable type '{type}'");
                }
            }
        }

        return new object();
    }



    public override object VisitDataType(CodeParser.DataTypeContext context)
    {
        if (context.INT_TYPE() != null)
        {
            // Handle integer data type
            return typeof(int);
        }
        else if (context.CHAR_TYPE() != null)
        {
            // Handle character data type
            return typeof(char);
        }
        else if (context.BOOL_TYPE() != null)
        {
            // Handle boolean data type
            return typeof(bool);
        }
        else if (context.FLOAT_TYPE() != null)
        {
            // Handle float data type
            return typeof(float);
        }
        else
        {
            // Invalid data type
            throw new ArgumentException("Invalid data type");
        }
    }

    public override object VisitAssignmentStatement([NotNull] CodeParser.AssignmentStatementContext context)
    {
        var varName = context.IDENTIFIER().GetText();
        var value = VisitExpression(context.expression());

        return Variables[varName] = value;
    }

    public override object VisitDisplayStatement([NotNull] CodeParser.DisplayStatementContext context)
    {
        foreach (var variable in Variables)
        {
            Console.Write(variable.Value + " ");
        }

        var displayValues = context.expression();
        foreach (var displayValue in displayValues)
        {
            var value = displayValue.GetText();
            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                Console.Write(value.Trim('"'));
            }
        }

        Console.WriteLine();

        return new object();
    }

    public override object VisitLiteral([NotNull] CodeParser.LiteralContext context)
    {
        if (context.INT_LITERAL() != null)
        {
            return int.Parse(context.INT_LITERAL().GetText());
        }
        else if (context.CHAR_LITERAL() != null)
        {
            string text = context.CHAR_LITERAL().GetText();
            // Remove the enclosing single quotes and escape sequences
            text = text.Substring(1, text.Length - 2).Replace("\\\\", "\\").Replace("\\\'", "\'");
            return char.Parse(text);
        }
        else if (context.FLOAT_LITERAL() != null)
        {
            return float.Parse(context.FLOAT_LITERAL().GetText());
        }
        else
        {
            throw new InvalidOperationException("Unknown Literal Type");
        }
    }

    public override object VisitLiteralExpression([NotNull] CodeParser.LiteralExpressionContext context)
    {
        if (context.literal().INT_LITERAL() is { } i)
        {
            return int.Parse(i.GetText());
        }
        else if (context.literal().FLOAT_LITERAL() is { } f)
        {
            return float.Parse(f.GetText());
        }
        else if(context.literal().CHAR_LITERAL() is { } c)
        {
            string text = c.GetText();
            //Remove quotation marks in char
            text = text.Substring(1, text.Length - 2).Replace("\\\\", "\\").Replace("\\\'", "\'");
            return char.Parse(text);
        }
        else
        {
            return new object();
        }
    }
}