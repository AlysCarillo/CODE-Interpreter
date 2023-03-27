using Antlr4.Runtime.Misc;
using CODE_Interpreter.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using static Antlr4.Runtime.Atn.SemanticContext;

public class CodeVisitor : CodeBaseVisitor<object>
{
    private Dictionary<string, object> Variables = new Dictionary<string, object>();

    //public CodeVisitor()
    //{
    //    Variables["DISPLAY:"] = new Func<object?[], object?>(Operator.Display);
    //    //Variable["SCAN:"] = new Func<object?[], object?>(Scan);
    //}

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
            Visit(statement);
        }
        return base.VisitProgram(context);
    }

    public override object VisitVariableDeclaration([NotNull] CodeParser.VariableDeclarationContext context)
    {
        //extract the variable name
        string variableName = context.IDENTIFIER().GetText();

        //extract the variable value
        object variableValue = Visit(context.expression());

        //update the variable in the symbol table
        Variables[variableName] = variableValue;
        return base.VisitVariableDeclaration(context);
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
        //extract the variable name
        string variableName = context.IDENTIFIER().GetText();

        //extract the variable value
        object variableValue = Visit(context.expression());

        //update the variable in the symbol table
        Variables[variableName] = variableValue;
        return base.VisitAssignmentStatement(context);
    }

}