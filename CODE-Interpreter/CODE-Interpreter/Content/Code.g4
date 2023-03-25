grammar Code;

// Lexical rules

BEGIN_CODE : NEWLINE? BEGIN CODE;
END_CODE : NEWLINE? END CODE EOF;
COMMENT : '#' ~[\r\n]* -> skip;

SCAN : 'SCAN';  
DISPLAY : 'DISPLAY';

IF : 'IF';
ELSE : 'ELSE';
WHILE : 'WHILE';
BEGIN : 'BEGIN';
CODE : 'CODE';
END : 'END';

LPAREN : '(';
RPAREN : ')';
COMMA : ',';
DOT : '.';
COLON : ':';
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

TRUE : 'true';
FALSE : 'false';

INT_TYPE : 'INT';
CHAR_TYPE : 'CHAR';
BOOL_TYPE : 'BOOL';
FLOAT_TYPE : 'FLOAT';

IDENTIFIER : [a-zA-Z_] [a-zA-Z0-9_]*;

INT_LITERAL : [0-9]+;
FLOAT_LITERAL : [0-9]+ DOT [0-9]+;
CHAR_LITERAL : '\'' ~('\'' | '\r' | '\n') '\'';

NEWLINE: '\n';

WHITESPACE : [ \t\r\n] -> skip;

// Parser rules

program : BEGIN_CODE variableDeclaration* statement* END_CODE;

variableDeclaration : dataType variableList SEMICOLON;

dataType : INT_TYPE | CHAR_TYPE | BOOL_TYPE | FLOAT_TYPE;

variableList : IDENTIFIER (COMMA IDENTIFIER)*;

statement : displayStatement
          | scanStatement
          | ifStatement
          | whileStatement
          | assignmentStatement
          | SEMICOLON;

displayStatement : DISPLAY expression SEMICOLON;

scanStatement : SCAN COLON variableList SEMICOLON;

ifStatement : IF LPAREN boolExpression RPAREN BEGIN statement* END
            (ELSE IF LPAREN boolExpression RPAREN BEGIN statement* END)*
            (ELSE BEGIN statement* END)?;

whileStatement : WHILE LPAREN boolExpression RPAREN BEGIN statement* END;

assignmentStatement : IDENTIFIER COLON expression SEMICOLON;

expression : boolExpression | arithmeticExpression;

boolExpression : boolTerm ((OR | AND) boolTerm)*;

boolTerm : (NOT)? boolFactor;

boolFactor : LPAREN boolExpression RPAREN
           | boolLiteral
           | relationalExpression;

boolLiteral : TRUE | FALSE;

relationalExpression : arithmeticExpression (GT | LT | GEQ | LEQ | EQ | NEQ) arithmeticExpression;

arithmeticExpression : term ((PLUS | MINUS) term)*;

term : factor ((MULTIPLY | DIVIDE | MODULO) factor)*;

factor : (PLUS | MINUS)? (INT_LITERAL | FLOAT_LITERAL | IDENTIFIER | LPAREN arithmeticExpression RPAREN);

// Error handling

UNEXPECTED_CHAR : .;
