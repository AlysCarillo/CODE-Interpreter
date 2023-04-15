grammar Code;

// Lexical rules

SCAN : 'SCAN: ';  
DISPLAY : 'DISPLAY';

IF : 'IF';
BEGIN_IF  : 'BEGIN IF';
END_IF : 'END IF';
ELSE : 'ELSE';
ELSE_IF : 'ELSE IF';
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
CONCAT: '&';

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

WHITESPACE : [ \t\r\n] -> skip;
COMMENT : '#' ~[\n]* -> skip;
NEWLINE: '\r' '\n' | '\r';

// Parser rules

program : NEWLINE? BEGIN NEWLINE statement* NEWLINE END;
line: (declaration | statement | COMMENT) NEWLINE;

statement : declaration
          | variable
          | variableAssignment
          | assignmentStatement
          | displayStatement
          | scanStatement
          | ifStatement
          | switchStatement
          | COMMENT
          ;

declaration : NEWLINE? dataType IDENTIFIER (ASSIGN expression)? (COMMA IDENTIFIER (ASSIGN expression)?)*; // INT x , y , z = 5
variableAssignment : NEWLINE? dataType IDENTIFIER (ASSIGN (expression))?; // INT x = 5
variable: NEWLINE? dataType IDENTIFIER NEWLINE?; // INT x 
variableDeclaration : declaration* NEWLINE?;

assignmentStatement : NEWLINE? IDENTIFIER (ASSIGN IDENTIFIER)* ASSIGN expression NEWLINE?; // x = y = z 

dataType : INT_TYPE | CHAR_TYPE | BOOL_TYPE | FLOAT_TYPE | STRING_TYPE;

literal :  INT_LITERAL
        |  CHAR_LITERAL
        |  FLOAT_LITERAL
        |  BOOL_LITERAL
        |  STRING_LITERAL
        ;

displayStatement : NEWLINE? DISPLAY':' expression NEWLINE?;
scanStatement : SCAN ':' IDENTIFIER (COMMA IDENTIFIER)* NEWLINE?;
ifStatement : NEWLINE? IF LPAREN (expression compareOP|boolOP expression) RPAREN NEWLINE BEGIN_IF NEWLINE statement* NEWLINE END_IF;

switchStatement : NEWLINE? 'SWITCH' expression NEWLINE
                 'BEGIN SWITCH' NEWLINE
                 (caseBlock)+
                 defaultBlock?
                 NEWLINE? 'END SWITCH'
                 ;

caseBlock : 'CASE' expression ':' statement* 'BREAK' NEWLINE?;

defaultBlock : 'DEFAULT' ':' statement* 'BREAK' NEWLINE?;

expression : literal                                #literalExpression
           | ESCAPE                                 #EscapeExpression                            
           | newlineOP                              #newlineExpression 
           | IDENTIFIER                             #identifierExpression
           | expression CONCAT expression 		    #concatExpression
           | LPAREN expression RPAREN               #parenthesisExpression
           | expression multOP expression           #multiplicationExpression
           | expression addOP expression            #additiveExpression
           | expression compareOP expression        #comparisonExpression
           | expression boolOP expression           #booleanExpression
           | unaryOP expression                     #unaryExpression
           | NOT expression                         #notExpression
           ;

multOP : MULTIPLY | DIVIDE | MODULO;
addOP : PLUS | MINUS;
compareOP : GT | LT | GEQ | LEQ | EQ | NEQ;
boolOP : AND | OR;
unaryOP: PLUS | MINUS;
newlineOP: '$';
ESCAPE: '['. ']';


// Error handling

UNEXPECTED_CHAR : .;