using Antlr4.Runtime.Misc;
using CODE_Interpreter.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using static Antlr4.Runtime.Atn.SemanticContext;
using System.Text;
using System.Reflection.Metadata.Ecma335;

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
                Console.Write("");
        }
        catch
        {
            Console.WriteLine("Code must start with 'BEGIN CODE' and end with 'END CODE'.");
            throw new ArgumentException("Code must start with 'BEGIN CODE' and end with 'END CODE'.");
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
        if (context.assignmentStatement() != null)
        {
            return VisitAssignmentStatement(context.assignmentStatement());
        }
        else if (context.displayStatement() != null)
        {
            return VisitDisplayStatement(context.displayStatement());
        }
        else if (context.variableDeclaration() != null)
        {
            return VisitVariableDeclaration(context.variableDeclaration());
        }
        else
        {
            throw new InvalidOperationException("Unknown Statement Type");
        }
    }

    public override object VisitVariableDeclaration([NotNull] CodeParser.VariableDeclarationContext context)
    {
        var type = context.dataType().GetText();
        //extract varName
        var varName = context.declaration().IDENTIFIER().Select(x => x.GetText()).ToList();

        //extract value of the variable
        var varValue = context.declaration().expression().Select(x => VisitExpression(x)).ToList();

        var varDictionary = new Dictionary<string, object>();
        for (int i = 0; i < varName.Count; i++)
        {
            if (Variables.ContainsKey(varName[i]))
            {
                Console.WriteLine($"Variable '{varName[i]}' is already defined!");
            }
            else
            {
                if (type.Equals("INT"))
                {
                    if (int.TryParse(varValue[i].ToString(), out int intValue))
                    {
                        varDictionary[varName[i]] = intValue as int?;
                    }
                    else
                    {
                        int value;
                        bool success = int.TryParse(varValue[i].ToString(), out value);
                        if (!success)
                        {
                            Console.WriteLine($"Invalid value for integer variable '{varName[i]}'");
                        }
                    }
                }
                else if (type.Equals("FLOAT"))
                {
                    //parse object varValue to object float varValue
                    if (float.TryParse(varValue[i].ToString(), out float floatValue))
                    {
                        varDictionary[varName[i]] = floatValue as float?;
                    }
                    else
                    {
                        float value;
                        bool success = float.TryParse(varValue[i].ToString(), out value);
                        if (!success)
                        {
                            Console.WriteLine($"Invalid value for float variable '{varName[i]}'");
                        }
                    }
                }
                else if (type.Equals("BOOL"))
                {
                    if (bool.TryParse(varValue[i].ToString(), out bool boolValue))
                        varDictionary[varName[i]] = boolValue;
                    else
                        Console.WriteLine($"Invalid value for boolean variable '{varName[i]}'");
                }
                else if (type.Equals("CHAR"))
                {
                    if (char.TryParse(varValue[i].ToString(), out char charValue))
                    {
                        varDictionary[varName[i]] = charValue;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid value for character variable '{varName[i]}'");
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid variable type '{type}'");
                }

                // add the value to Variables
                Variables[varName[i]] = varDictionary[varName[i]];
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
        var assignments = context.ASSIGN();
        var identifiers = context.IDENTIFIER();
        var expressions = context.expression();

        for (int i = 0; i < assignments.Length; i++)
        {
            var identifier = identifiers[i].GetText();
            var expression = expressions[i];
            if (Variables.ContainsKey(identifier))
            {
                Variables[identifier] = VisitExpression(expression);
            }
            else
            {
                Console.WriteLine($"Variable '{identifier}' is not defined!");
            }
        }

        return new object();

    }

    public override object VisitDisplayStatement([NotNull] CodeParser.DisplayStatementContext context)
    {
        //get the value of the variable from the dictionary
        //var value = context.expression().GetText();
        var valueContext = context.expression()[0];
        var value = valueContext.GetText();
        if (Variables.ContainsKey(value))
        {
            value = Variables[value].ToString();
        }
        else
        {
            Console.WriteLine($"Variable '{value}' is not defined!");
        }

        Console.Write(value);
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
        else if (context.literal().CHAR_LITERAL() is { } c)
        {
            string text = c.GetText();
            //Remove quotation marks in char
            text = text.Substring(1, text.Length - 2).Replace("\\\\", "\\").Replace("\\\'", "\'");
            return char.Parse(text);
        }

        return new object();
    }

    public override object VisitConcatExpression([NotNull] CodeParser.ConcatExpressionContext context)
    {
        var leftValue = Visit(context.expression()[0]);
        var rightValue = Visit(context.expression()[1]);
        return leftValue.ToString() + rightValue.ToString();
    }
}