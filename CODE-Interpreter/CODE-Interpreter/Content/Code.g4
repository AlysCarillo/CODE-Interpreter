grammar Code;

// Lexical rules

COMMENT : '#' ~[\r\n]* -> skip;

SCAN : 'SCAN: ';  
DISPLAY : 'DISPLAY';

IF : 'IF';
BEGIN_IF  : 'BEGIN IF';
END_IF : 'END IF';
ELSE : 'ELSE';
WHILE : 'WHILE';
BEGIN : 'BEGIN CODE';
CODE : 'CODE';
END : 'END CODE';

LPAREN : '(';
RPAREN : ')';
COMMA : ',';
DOT : '.';
COLON : ':';
ASSIGN : '=';
SEMICOLON : ';';
CONCAT : '&';
MULTIPLY : '*';
DIVIDE : '/';
MODULO : '%';
PLUS : '+';
MINUS : '-';
GT : '>';
LT : '<';
GEQ : '>=';
LEQ : '<=';
EQ : '==';
NEQ : '<>';
AND : 'AND';
OR : 'OR';
NOT : 'NOT';

TRUE : 'TRUE';
FALSE : 'FALSE';

INT_TYPE : 'INT';
CHAR_TYPE : 'CHAR';
BOOL_TYPE : 'BOOL';
FLOAT_TYPE : 'FLOAT';
STRING_TYPE : 'STRING';

IDENTIFIER : [a-zA-Z_] [a-zA-Z0-9_]*;

INT_LITERAL : [0-9]+;
FLOAT_LITERAL : [0-9]+ DOT [0-9]+;
CHAR_LITERAL : '\'' ~('\''|'\\') '\'';
BOOL_LITERAL : TRUE | FALSE;
STRING_LITERAL : ('"' ~'"'* '"') | ('\'' ~'\''* '\'');

NEWLINE: '\n';

WHITESPACE : [ \t\r\n] -> skip;

// Parser rules

program : BEGIN NEWLINE statement* NEWLINE END;

statement : assignmentStatement
          | variableDeclaration
          | displayStatement
          | scanStatement
          ;


declaration : IDENTIFIER ((ASSIGN IDENTIFIER)* (ASSIGN expression))? (COMMA IDENTIFIER (ASSIGN expression)?)* ;
variableDeclaration : dataType declaration NEWLINE?;

assignmentStatement : (IDENTIFIER ASSIGN)+ expression?;

line: (variableDeclaration | statement | COMMENT) NEWLINE;

dataType : INT_TYPE | CHAR_TYPE | BOOL_TYPE | FLOAT_TYPE | STRING_TYPE;
variableList : IDENTIFIER ((ASSIGN IDENTIFIER)* (ASSIGN expression))? (COMMA IDENTIFIER (ASSIGN expression)?)* ;

literal :  INT_LITERAL
        |  CHAR_LITERAL
        |  FLOAT_LITERAL
        |  BOOL_LITERAL
        |  STRING_LITERAL
        ;

displayStatement : NEWLINE? DISPLAY':' (expression | variableDeclaration) NEWLINE?;
scanStatement : SCAN (IDENTIFIER (COMMA IDENTIFIER)*)* NEWLINE;


expression : literal                                #literalExpression
           | IDENTIFIER                             #identifierExpression
           | LPAREN expression RPAREN               #parenthesisExpression
           | expression multOP expression           #multiplicationExpression
           | expression addOP expression            #additionExpression
           | expression compareOP expression        #comparisonExpression
           | expression boolOP expression           #booleanExpression
           | unaryOP expression                     #unaryExpression
           | NOT expression                         #notExpression
           | expression CONCAT expression 		    #concatExpression
           ;

multOP : MULTIPLY | DIVIDE | MODULO;
addOP : PLUS | MINUS;
compareOP : GT | LT | GEQ | LEQ | EQ | NEQ;
boolOP : AND | OR;
unaryOP: PLUS | MINUS;

// Error handling

UNEXPECTED_CHAR : .;