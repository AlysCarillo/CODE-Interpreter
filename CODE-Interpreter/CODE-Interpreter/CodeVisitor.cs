using Antlr4.Runtime.Misc;
using CODE_Interpreter.Content;
using System;
using System.Collections.Generic;

public class CodeVisitor : CodeBaseVisitor<object>
{
    private Dictionary<string, object> symbolTable = new Dictionary<string, object>();

    public override object VisitProgram(CodeParser.ProgramContext context)
    {
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

    //WA NAKO KASABOT HAHAHAHAHA
    public override object VisitVariableDeclaration(CodeParser.VariableDeclarationContext context)
    {
        // Get the variable name
        string variableName = context.IDENTIFIER().GetText();

        // Get the variable value
        object variableValue = Visit(context.expression());

        // Add the variable to the symbol table
        symbolTable.Add(variableName, variableValue);

        return base.VisitVariableDeclaration(context);
    }

}