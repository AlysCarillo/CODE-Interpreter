grammar Code;

// Lexical rules

COMMENT : '#' ~[\r\n]* -> skip;

SCAN : 'SCAN: ';  
DISPLAY : 'DISPLAY: ';

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

IDENTIFIER : [a-zA-Z_] [a-zA-Z0-9_]*;

INT_LITERAL : [0-9]+;
FLOAT_LITERAL : [0-9]+ DOT [0-9]+;
CHAR_LITERAL : '\'' ~('\''|'\\') '\'';
BOOL_LITERAL : TRUE | FALSE;

NEWLINE: '\n';

WHITESPACE : [ \t\r\n] -> skip;

// Parser rules

program : BEGIN NEWLINE variableDeclaration* statement* NEWLINE END;
variableDeclaration : dataType variableList* (ASSIGN expression)? NEWLINE;
line: (variableDeclaration | statement | COMMENT) NEWLINE;

dataType : INT_TYPE | CHAR_TYPE | BOOL_TYPE | FLOAT_TYPE;
variableList : IDENTIFIER (COMMA IDENTIFIER)*;

literal :  INT_LITERAL
        |  CHAR_LITERAL
        |  FLOAT_LITERAL
        |  BOOL_LITERAL
        ;

statement : assignmentStatement
          | displayStatement
          | scanStatement
          | variableDeclaration
          ;

displayStatement : DISPLAY expression*;
scanStatement : SCAN (IDENTIFIER (COMMA IDENTIFIER)*)* NEWLINE;

//ifStatement : IF LPAREN expression RPAREN IF_BLOCK (ELSE IFELSE_BLOCK);

//IF_BLOCK : BEGIN_IF statement* END_IF;

//IFELSE_BLOCK : BLOCK | IF_BLOCK;

//BLOCK : 'LATUR';

// whileStatement : WHILE LPAREN boolExpression RPAREN BEGIN statement* END;

assignmentStatement : dataType IDENTIFIER ASSIGN expression NEWLINE; 

expression : literal                                #literalExpression
           | IDENTIFIER                             #identifierExpression
           | LPAREN expression RPAREN               #parenthesisExpression
           | expression multOP expression           #multiplicationExpression
           | expression addOP expression            #additionExpression
           | expression compareOP expression        #comparisonExpression
           | expression boolOP expression           #booleanExpression
           ;

multOP : MULTIPLY | DIVIDE | MODULO;
addOP : PLUS | MINUS;
compareOP : GT | LT | GEQ | LEQ | EQ | NEQ;
boolOP : AND | OR;


// Error handling

UNEXPECTED_CHAR : .;