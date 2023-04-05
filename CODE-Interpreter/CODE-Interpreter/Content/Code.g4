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

ESCAPE_SEQUENCE: '[' . ']';

// Parser rules

program : NEWLINE? BEGIN NEWLINE statement* NEWLINE END;
line: (declaration | statement | COMMENT) NEWLINE;

statement : assignmentStatement
          | displayStatement
          | scanStatement
          | declaration
          | variable
          | variableAssignment
          | COMMENT
          ;

declaration : NEWLINE? dataType IDENTIFIER (ASSIGN expression)? (COMMA IDENTIFIER (ASSIGN expression)?)*; // INT x , y , z = 5
variable : NEWLINE? dataType IDENTIFIER (ASSIGN (expression))?; // INT x = 5
variableAssignment: NEWLINE? dataType IDENTIFIER NEWLINE?; // INT x 
variableDeclaration : declaration* NEWLINE?;

assignmentStatement : NEWLINE? IDENTIFIER (ASSIGN IDENTIFIER)* ASSIGN expression NEWLINE?; // x = y = z 

dataType : INT_TYPE | CHAR_TYPE | BOOL_TYPE | FLOAT_TYPE | STRING_TYPE;

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
           | ESCAPE_SEQUENCE                        #escapeSequenceExpression
           ;

multOP : MULTIPLY | DIVIDE | MODULO;
addOP : PLUS | MINUS;
compareOP : GT | LT | GEQ | LEQ | EQ | NEQ;
boolOP : AND | OR;
unaryOP: PLUS | MINUS;

// Error handling

UNEXPECTED_CHAR : .;