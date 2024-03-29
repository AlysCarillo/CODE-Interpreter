using Antlr4.Runtime.Misc;
using CODE_Interpreter.Content;
using CODE_Interpreter.Functions;
using CODE_Interpreter.ErrorHandling;

public class CodeVisitor : CodeBaseVisitor<object>
{
    private Dictionary<string, object> Variables = new Dictionary<string, object>();
    private Dictionary<string, object> DataTypes = new Dictionary<string, object>();

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
        else if (context.scanStatement() != null)
        {
            return VisitScanStatement(context.scanStatement());
        }
        else if (context.COMMENT() != null)
        {
            return new object();
        }
        else if (context.ifStatement() != null)
        {
            return VisitIfStatement(context.ifStatement());
        }
        else if (context.switchStatement() != null)
        {
            return VisitSwitchStatement(context.switchStatement());
        }
        else if (context.forStatement() != null)
        {
            return VisitForStatement(context.forStatement());
        }
        else if (context.whileStatement() != null)
        {
            return VisitWhileStatement(context.whileStatement());
        }
        else
        {
            throw new InvalidOperationException("Unknown Statement Type");
        }
    }

    public override List<object?> VisitDeclaration([NotNull] CodeParser.DeclarationContext context)
    {
        var type = Visit(context.dataType());
        var typeStr = context.dataType().GetText();
        var varnames = context.IDENTIFIER();

        // remove type
        var contextValue = context.GetText().Replace(typeStr, "");

        var contextArray = contextValue.Split(',');
        var exp = context.expression();
        int expctr = 0;

        //traverse each part
        for (int x = 0; x < contextArray.Length; x++)
        {
            if (Variables.ContainsKey(varnames[x].GetText()))
            {
                Console.WriteLine(varnames[x].GetText() + " is already declared");
                continue;
            }

            if (contextArray[x].Contains('='))
            {
                if (expctr < exp.Count())
                {
                    // check type
                    if (ErrorHandler.HandleTypeError(context, Visit(exp[expctr]), (Type?)type, "Variable Declaration"))
                    {
                        Variables[varnames[x].GetText()] = Visit(exp[expctr]);
                        DataTypes[varnames[x].GetText()] = type;
                    }

                    expctr++;
                }
            }
            else
            {
                Variables[varnames[x].GetText()] = new object();
                DataTypes[varnames[x].GetText()] = type;
            }

        }

        return new List<object?>();
    }

    public override object VisitVariable([NotNull] CodeParser.VariableContext context)
    {
        var dataType = context.dataType().GetText();
        var varName = context.IDENTIFIER().GetText();

        return (Variables[varName] = new object(), DataTypes[varName] = dataType);
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

        return (Variables[varName] = exp, DataTypes[varName] = type);
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
            // Handle string data type
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
            // check type
            if (ErrorHandler.HandleTypeError(context, expression, (Type?)DataTypes[i.GetText()], "Variable Assignment"))
            {
                Variables[i.GetText()] = expression;
            }
        }

        return new object();
    }

    public override object VisitDisplayStatement([NotNull] CodeParser.DisplayStatementContext context)
    {
        var exp = Visit(context.expression());

        if (exp is bool b)
            exp = b.ToString().ToUpper();

        Console.Write(exp);

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
        else if (context.BOOL_LITERAL() != null)
        {
            return bool.Parse(context.BOOL_LITERAL().GetText()).Equals("\"TRUE\"");
        }
        else if (context.STRING_LITERAL() != null)
        {
            return context.STRING_LITERAL().GetText()[1..^1];
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
            return b.GetText().Equals("\"TRUE\"");
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
            Console.WriteLine($"SYNTAX ERROR: Variable {identifier} is not declared");
            Environment.Exit(400);
        }
        return new object();
    }

    public override object VisitNewlineExpression([NotNull] CodeParser.NewlineExpressionContext context)
    {
        return "\n";
    }

    public override object VisitAdditiveExpression([NotNull] CodeParser.AdditiveExpressionContext context)
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

    public override object VisitParenthesisExpression([NotNull] CodeParser.ParenthesisExpressionContext context)
    {
        return Visit(context.expression());
    }

    public override object VisitScanStatement([NotNull] CodeParser.ScanStatementContext context)
    {
        var input = Console.ReadLine();
        var inputs = input!.Split(',').Select(s => s.Trim()).ToArray();

        if (inputs.Length < 1 || inputs.Length > context.IDENTIFIER().Length)
        {
            Console.WriteLine($"Invalid number of inputs. Expected {context.IDENTIFIER().Length}, but got {inputs.Length}.");
            Environment.Exit(400);
        }

        for (int i = 0; i < inputs.Length; i++)
        {
            var idName = context.IDENTIFIER(i).GetText();
            if (!Variables.ContainsKey(idName))
            {
                Console.WriteLine($"SYNTAX ERROR: Variable {idName} is not declared");
                Environment.Exit(400);
            }

            Type? variableType = (Type?)DataTypes[idName];

            try
            {
                object? convertedValue = Convert.ChangeType(input, variableType!);
                return Variables[idName] = convertedValue;
            }
            catch (Exception e)
            {
                Console.WriteLine($"TYPE MISMATCH: Cannot assign {input} to variable {idName} of type {variableType?.Name}");
                Environment.Exit(400);
            }

        }
        return new object();
    }

    public override object VisitEscapeExpression([NotNull] CodeParser.EscapeExpressionContext context)
    {
        return context.ESCAPE().GetText()[1];
    }

    public override object VisitIfStatement([NotNull] CodeParser.IfStatementContext context)
    {
        var condition = (bool)(Visit(context.expression()));

        if (condition)
        {
            // Execute the statements inside the if block
            foreach (var statement in context.statement())
            {
                VisitStatement(statement);
            }
            return new object();
        }

        foreach (var elseIfBlock in context.elseIfBlock())
        {
            var elseIfCondition = (bool)Visit(elseIfBlock.expression());
            if (elseIfCondition)
            {
                foreach (var statement in elseIfBlock.statement())
                {
                    VisitStatement(statement);
                }
                return new object();
            }
        }

        if(context.elseBlock() != null)
        {
            foreach (var statement in context.elseBlock().statement())
            {
                VisitStatement(statement);
                return new object();
            }
        }
        
        return new object();
    }

    public override object VisitSwitchStatement([NotNull] CodeParser.SwitchStatementContext context)
    {
        //get the expression in which the switch would use as a base for evaluation
        var expr = Visit(context.expression());
        //this boolean is for flagging whenever a correct case is found
        bool flag = false;

        //loop all the given case blocks
        foreach (var caseBlock in context.caseBlock())
        {
            var caseValue = Visit(caseBlock.expression());

            //finding the case where expr satisfies the caseValue and visit that case block
            if (caseValue is bool && Convert.ToBoolean(caseValue) || caseValue.Equals(expr))
            {
                Visit(caseBlock);
                flag = true;
            }
        }

        //if no satisfactory case block is found, default block is then automatically run
        if (context.defaultBlock() != null && flag == false)
        {
            Visit(context.defaultBlock());
        }

        return new object();
    }

    public override object VisitForStatement([NotNull] CodeParser.ForStatementContext context)
    {
        // Get the initialization, condition, and increment statements
        var statement = context.statement();
        var conditionExpr = context.expression();
        var increment = context.assignmentStatement();

        // Get all the lines after BEGIN FOR and before END FOR
        var lines = context.line();

        // Visit the loop initialization statement
        Visit(statement);

        // Evaluate the loop condition expression
        bool loopCondition = Convert.ToBoolean(Visit(conditionExpr));

        // Flag to track if the loop has executed at least once
        bool executedOnce = false;

        // Evaluate the condition expression and continue while it's true
        while (loopCondition)
        {
            // Check if the loop has executed at least once
            if (executedOnce)
            {
                Console.WriteLine("SYNTAX ERROR: Infinite loop");
                break;
            }

            // Set the flag to indicate the loop has executed
            executedOnce = true;

            // Evaluate the body of the for loop
            foreach (var line in lines)
            {
                Visit(line);
            }

            // Evaluate the increment statement
            Visit(increment);

            // Evaluate the loop condition expression again
            loopCondition = Convert.ToBoolean(Visit(conditionExpr));
        }

        return new object();
    }

    public override object VisitWhileStatement([NotNull] CodeParser.WhileStatementContext context)
    {
        int maxIterations = 1000; 
        int iterationCount = 0; 

        while ((bool)Visit(context.expression()))
        {
            iterationCount++;

            // Check if the iteration count exceeds the maximum threshold
            if (iterationCount > maxIterations)
            {
                Console.WriteLine("SYNTAX ERROR: Infinite loop");
                break;
            }

            // Execute the statements inside the while block
            foreach (var statement in context.statement())
            {
                VisitStatement(statement);
            }
        }

        return new object();
    }
}