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
using CODE_Interpreter;

public class CodeVisitor : CodeBaseVisitor<object>
{
    private Dictionary<string, object> Variables = new Dictionary<string, object>();
    private Operators op = new Operators();

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
        // Extract variable data type
        var type = context.dataType().GetText();
        var varName = context.declaration().IDENTIFIER().Select(x => x.GetText()).ToList();

        var declaration = context.declaration().GetText().Split(',');
        var exp = context.declaration().expression();
        int flagExp = 0;

        for (int i = 0; i < declaration.Length; i++)
        {
            if (Variables.ContainsKey(varName[i]))
            {
                Console.WriteLine(varName[i] + "is already declared");
                continue;
            }
            if (declaration[i].Contains('='))
            {
                if (flagExp < exp.Count())
                {
                    Variables[varName[i]] = Visit(exp[flagExp]);
                    flagExp++;
                }
            }
            else
            {
                // Initialize variable with default value
                switch (type)
                {
                    case "INT":
                        Variables[varName[i]] = 0;
                        break;
                    case "FLOAT":
                        Variables[varName[i]] = 0.0f;
                        break;
                    case "STRING":
                        Variables[varName[i]] = "";
                        break;
                    case "BOOL":
                        Variables[varName[i]] = false;
                        break;
                    default:
                        Variables[varName[i]] = new object();
                        break;
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
        else if(context.STRING_TYPE() != null)
        {
            return typeof(string);
        }
        else
        {
            // Invalid data type
            throw new ArgumentException("Invalid data type");
        }
    }

    public override object VisitAssignmentStatement([NotNull] CodeParser.AssignmentStatementContext context)
    {
        var identifier = context.IDENTIFIER();
        foreach (var i in identifier)
        {
            var expression = context.expression().Accept(this);
            Variables[i.GetText()] = expression;
        }

        return new object();
    }

    public override object VisitDisplayStatement([NotNull] CodeParser.DisplayStatementContext context)
    {
        //get the value of the variable from the dictionary
        //var value = context.expression().GetText();
        var valueContext = context.expression();
        var value = valueContext.GetText();
        if (Variables.ContainsKey(value))
        {
            value = Variables[value].ToString();
        }
        else if (value.Contains('\"'))
        {
            value = value.Replace("\"", "");
        }
        else
        {
            Console.WriteLine($"Variable '{value}' is not defined!");
        }

        Console.Write(value + " ");
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
        else if(context.STRING_LITERAL() != null)
        {
            string text = context.STRING_LITERAL().GetText();
            // Remove the enclosing double quotes and escape sequences
            text = text.Substring(1, text.Length - 2).Replace("\\\\", "\\").Replace("\\\"", "\"");
            return text;
        }
        else if(context.BOOL_LITERAL() != null)
        {
            return bool.Parse(context.BOOL_LITERAL().GetText());
        }
        else
        {
            throw new InvalidOperationException("Unknown Literal Type");
        }
    }

    public override object VisitUnaryExpression([NotNull] CodeParser.UnaryExpressionContext context)
    {
        return op.Unary(context.unaryOP().GetText(), Visit(context.expression()));
    }

    public override object VisitConcatExpression([NotNull] CodeParser.ConcatExpressionContext context)
    {
        // Visit the left and right expressions
        var left = context.expression()[0].Accept(this);
        var right = context.expression()[1].Accept(this);

        // Check if both left and right are variable names
        if (left == null && right == null)
        {
            throw new NullReferenceException();
        }

        // Concatenate the left and right values
        var output = "";
        if (left != null)
        {
            output += left.ToString();
        }
        if (right != null)
        {
            output += right.ToString();
        }

        return output;
    }

    public override object VisitVariableAssignment([NotNull] CodeParser.VariableAssignmentContext context)
    {
        var dataType = context.dataType().GetText();
        var varName = context.IDENTIFIER().GetText();

        return Variables[varName] = new object();
    }

    public override object VisitIdentifierExpression([NotNull] CodeParser.IdentifierExpressionContext context)
    {
        var identifier = context.IDENTIFIER().GetText();
        if (Variables.ContainsKey(identifier))
        {
            return Variables[identifier];
        }
        else
        {
            throw new Exception($"Variable {identifier} is not declared");
        }
    }

}