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
        else if (context.variableAssignment() != null)
        {
            return VisitVariableAssignment(context.variableAssignment());
        }
        else if (context.declaration() != null)
        {
            return VisitDeclaration(context.declaration());
        }
        else if (context.variable() != null)
        {
            return VisitVariable(context.variable());
        }
        else
        {
            throw new InvalidOperationException("Unknown Statement Type");
        }
    }

    public override List<object?> VisitDeclaration([NotNull] CodeParser.DeclarationContext context)
    {
        var type = context.dataType().GetText();
        var varnames = context.IDENTIFIER();

        // remove type
        var contextstring = context.GetText().Replace(type, "");

        var contextParts = contextstring.Split(',');
        var exp = context.expression();
        int expctr = 0;

        // traverse each part
        for (int x = 0; x < contextParts.Length; x++)
        {
            if (Variables.ContainsKey(varnames[x].GetText()))
            {
                Console.WriteLine(varnames[x].GetText() + "is already declared");
                continue;
            }
            if (contextParts[x].Contains('='))
            {
                if (expctr < exp.Count())
                {
                    Variables[varnames[x].GetText()] = Visit(exp[expctr]);
                    expctr++;
                }
            }
            else
            {
                Variables[varnames[x].GetText()] = new object();
            }

        }

        return new List<object?>();
    }

    public override object VisitVariable([NotNull] CodeParser.VariableContext context)
    {
        var dataType = context.dataType().GetText();
        var varName = context.IDENTIFIER().GetText();

        return Variables[varName] = new object();
    }

    public override object VisitVariableAssignment([NotNull] CodeParser.VariableAssignmentContext context)
    {
        var type = context.dataType().GetText();
        var varName = context.IDENTIFIER().GetText();
        var exp = context.expression();

        if (Variables.ContainsKey(varName))
        {
            Console.WriteLine(varName + "is already declared");
            return new object();
        }

        return Variables[varName] = exp;
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
        else if (context.STRING_TYPE() != null)
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
        var exp = Visit(context.expression());

        Console.Write(exp + " ");

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
        else if (context.STRING_LITERAL() != null)
        {
            string text = context.STRING_LITERAL().GetText();
            // Remove the enclosing double quotes and escape sequences
            text = text.Substring(1, text.Length - 2).Replace("\\\\", "\\").Replace("\\\"", "\"");
            return text;
        }
        else if (context.BOOL_LITERAL() != null)
        {
            return bool.Parse(context.BOOL_LITERAL().GetText());
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
        else if (context.literal().BOOL_LITERAL() is { } b)
        {
            return bool.Parse(b.GetText().ToString().ToUpper());
        }
        else if (context.literal().STRING_LITERAL() is { } s)
        {
            string text = s.GetText();
            text = text.Substring(1, text.Length - 2).Replace("\\\\", "\\").Replace("\\\"", "\"");
            return text;

        }

        return new object();
    }

    public override object VisitUnaryExpression([NotNull] CodeParser.UnaryExpressionContext context)
    {
        return Operators.Unary(context.unaryOP().GetText(), Visit(context.expression()));
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

    public override object VisitNewlineExpression([NotNull] CodeParser.NewlineExpressionContext context)
    {
        var leftExpression = Visit(context.expression(0));
        var rightExpression = Visit(context.expression(1));
        return op.NewlineSymbol($"{leftExpression}{Environment.NewLine}{rightExpression}");
    }

    public override object VisitAdditionExpression([NotNull] CodeParser.AdditionExpressionContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));
        var addop = context.addOP().GetText();

        return addop switch
        {
            "+" => Operators.Add(left, right),
            "-" => Operators.Subtract(left, right),
            _ => throw new InvalidOperationException("Unknown operator")
        };
    }

    public override object VisitMultiplicationExpression([NotNull] CodeParser.MultiplicationExpressionContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));
        var multop = context.multOP().GetText();

        return multop switch
        {
            "*" => Operators.Multiply(left, right),
            "/" => Operators.Divide(left, right),
            "%" => Operators.Modulo(left, right),
            _ => throw new InvalidOperationException("Unknown operator")
        };
    }

    public override object VisitComparisonExpression([NotNull] CodeParser.ComparisonExpressionContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));
        var compop = context.compareOP().GetText();

        return compop switch
        {
            "==" => Operators.Equal(left, right),
            "<>" => Operators.NotEqual(left, right),
            ">" => Operators.GreaterThan(left, right),
            ">=" => Operators.GreaterThanEqual(left, right),
            "<" => Operators.LesserThan(left, right),
            "<=" => Operators.LesserThanEqual(left, right),
            _ => throw new InvalidOperationException("Unknown operator")
        };
    }

    public override object VisitBooleanExpression([NotNull] CodeParser.BooleanExpressionContext context)
    {
        var left = Visit(context.expression(0));
        var right = Visit(context.expression(1));
        var boolop = context.boolOP().GetText();

        return boolop switch
        {
            "AND" => Operators.AndBoolean(left, right),
            "OR" => Operators.OrBoolean(left, right),
            _ => throw new InvalidOperationException("Unknown operator")
        };
    }

    public override object VisitNotExpression([NotNull] CodeParser.NotExpressionContext context)
    {
        var expression = Visit(context.expression());
        return Operators.NotBoolean(expression);    
    }
}