
%{
/*
 * scan.l
 *
 * lex input file for pascal scanner
 *
 * extensions: to ways to spell "external" and "->" ok for "^".
 */

#include "service.h"
#include "parser.hpp"
int line_no = 1;

%}

%x commentA
%x commentB

A       [aA]
B       [bB]
C       [cC]
D       [dD]
E       [eE]
F       [fF]
G       [gG]
H       [hH]
I       [iI]
J       [jJ]
K       [kK]
L       [lL]
M       [mM]
N       [nN]
O       [oO]
P       [pP]
Q       [qQ]
R       [rR]
S       [sS]
T       [tT]
U       [uU]
V       [vV]
W       [wW]
X       [xX]
Y       [yY]
Z       [zZ]
NQUOTE  [^']

White             [ \t\r\f\v]
CommentAStart      \(\*
CommentAEnd        \*\)
CommentBStart      \{
CommentBEnd        \}
LineComment		   \/\/

Hint			  {White}*[A-Z0-9_]+{White}*:.*\n
       
%%

{A}{N}{D}						return(KWAND);
{A}{R}{R}{A}{Y} 				return(KWARRAY);
{C}{A}{S}{E}					return(KWCASE);
{C}{O}{N}{S}{T} 				return(KWCONST);
{D}{I}{V}						return(KWDIV);
{D}{O}							return(KWDO);
{D}{O}{W}{N}{T}{O}				return(KWDOWNTO);
{E}{L}{S}{E}					return(KWELSE);
{E}{N}{D}						return(KWEND);
{E}{X}{T}{E}{R}{N} |
{E}{X}{T}{E}{R}{N}{A}{L}		return(KWEXTERNAL);
{F}{O}{R}						return(KWFOR);
{F}{O}{R}{W}{A}{R}{D}			return(KWFORWARD);
{F}{U}{N}{C}{T}{I}{O}{N}		return(KWFUNCTION);
{G}{O}{T}{O}					return(KWGOTO);
{I}{F}							return(KWIF);
{I}{N}							return(KWIN);
{L}{A}{B}{E}{L} 				return(KWLABEL);
{M}{O}{D}						return(KWMOD);
{N}{I}{L}						return(KWNIL);
{N}{O}{T}						return(KWNOT);
{O}{F}							return(KWOF);
{O}{R}							return(KWOR);
{O}{T}{H}{E}{R}{W}{I}{S}{E}     return(KWOTHERWISE);
{P}{A}{C}{K}{E}{D}              return(KWPACKED);
{B}{E}{G}{I}{N} 				return(KWBEGIN);
{F}{I}{L}{E}					return(KWFILE);
{P}{R}{O}{C}{E}{D}{U}{R}{E} 	return(KWPROCEDURE);
{P}{R}{O}{G}{R}{A}{M}			return(KWPROGRAM);
{R}{E}{C}{O}{R}{D}				return(KWRECORD);
{R}{E}{P}{E}{A}{T}				return(KWREPEAT);
{S}{E}{T}						return(KWSET);
{T}{H}{E}{N}					return(KWTHEN);
{T}{O}							return(KWTO);
{T}{Y}{P}{E}					return(KWTYPE);
{U}{N}{T}{I}{L} 				return(KWUNTIL);
{V}{A}{R}						return(KWVAR);
{W}{H}{I}{L}{E} 				return(KWWHILE);
{W}{I}{T}{H}					return(KWWITH);

{I}{M}{P}{O}{R}{T}				return(KWIMPORT);

[a-zA-Z]([a-zA-Z0-9_])*			return(IDENTIFIER);


":="							return(ASSIGNMENT);
'({NQUOTE}|'')+'				return(CHARACTER_STRING);
":" 							return(COLON);
"," 							return(COMMA);
[0-9]+							return(DIGSEQ);
"." 							return(DOT);
".."							return(DOTDOT);
"=" 							return(EQUAL);
">="							return(GE);
">" 							return(GT);
"[" 							return(LBRAC);
"<="							return(LE);
"(" 							return(LPAREN);
"<" 							return(LT);
"-" 							return(MINUS);
"<>"							return(NOTEQUAL);
"+" 							return(PLUS);
"]" 							return(RBRAC);
[0-9]+"."[0-9]+ 				return(REALNUMBER);
")" 							return(RPAREN);
";" 							return(SEMICOLON);
"/" 							return(SLASH);
"*" 							return(STAR);
"**"							return(STARSTAR);
"->" |
"^" 							return(UPARROW);

{CommentAStart}					{ g_service->enterComment( commentA );
									yymore();
								}

{LineComment}{Hint}				{ g_service->lexicalError( SevHint, yytext+2 ); 
                                  return LEX_LINE_COMMENT; 
                                }
{LineComment}.*\n				{ return LEX_LINE_COMMENT; }

<commentA>{CommentAStart}		{ g_service->enterComment();
									yymore();
								}
<commentA>{CommentAEnd}			{ g_service->leaveComment();
									return LEX_COMMENT;
								}
<commentA>{CommentBEnd}			{ g_service->leaveComment();
									return LEX_COMMENT;
								}
<commentA>.						{ yymore(); }
<commentA>\n					{ return LEX_COMMENT; 
								  line_no++; }

{CommentBStart}								{ g_service->enterComment( commentB );
									yymore();
								}
<commentB>{CommentBStart}					{ g_service->enterComment();
									yymore();
								}
<commentB>{CommentBEnd}			{ g_service->leaveComment();
									return LEX_COMMENT;
								}
<commentB>{CommentAEnd}			{ g_service->leaveComment();
									return LEX_COMMENT;
								}
<commentB>.						{ yymore(); }
<commentB>\n					{ return LEX_COMMENT; 
								  line_no++; }



{White}							return(LEX_WHITE);

\n								{ line_no++;
								  return(LEX_WHITE);
								}

.								{ fprintf (stderr,
									"'%c' (0%o): illegal charcter at line %d\n",
									yytext[0], yytext[0], line_no);
								}

%%



#include <stdservice.c>
