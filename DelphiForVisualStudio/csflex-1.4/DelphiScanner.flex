/* This file is part of "Delphi For Visual Studio" project
 * http://www.codeplex.com/Delphi4VisualStudio
 * Copyright (c) 2006 Davinci Jeremie. All rights reserved.
 * Created 2007/04/20 by Davinci Jeremie (Web: http://www.Jeremie.ca )
 * 
 * LICENSE: LGPL
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License (LGPL) as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * For more details on the GNU Lesser General Public License,
 * see http://www.gnu.org/copyleft/lesser.html
 *************************************************************************/
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.VisualStudio.Package;

namespace VisualStudio.Delphi.Language
{

%%

/* Class is internal */
/* %public   */
%class DelphiScanner
%implements IScanner
%function GetNextToken
%type TokenInfo
/* scanning Unicode documents */
%unicode

%column
%line
%eofclose
%caseless
%ignorecase

%state COMMENT1, COMMENT2, ASSEMBLER

/* we want to return the following items...
    // Summary:
    //     The token is an unknown type. This is typically used for any token not recognized
    //     by the parser and should be considered an error in the code being parsed.
    Unknown = 0,
    //
    // Summary:
    //     General text; any text not identified as a specified token type.
    Text = 1,
    //
    // Summary:
    //     A language keyword, an identifier that is reserved by the language. For example,
    //     in C#, do, while, foreach, if, and else, are all keywords.
    Keyword = 2,
    //
    // Summary:
    //     An identifier or name. For example, the name of a variable, method, or class.
    //     In XML, this could be the name of a tag or attribute.
    Identifier = 3,
    //
    // Summary:
    //     A string. Typically defined as zero or more characters bounded by double
    //     quotes.
    String = 4,
    //
    // Summary:
    //     A literal value (a character or number). For example, in C# or C++, this
    //     is a character bounded by single quotes, or a decimal or hexadecimal number.
    Literal = 5,
    //
    // Summary:
    //     A punctuation character that has a specific meaning in a language. For example,
    //     in C#, arithmetic operators +, -, *, and /. In C++, pointer dereference operator
    //     ->, insertion operator >>, and extraction operation <<. In XML, assignment
    //     operator =.
    Operator = 6,
    //
    // Summary:
    //     A token that operates as a separator between two language elements. For example,
    //     in C#, the period "." between class name and member name. In XML, the angle
    //     brackets surrounding a tag, < and >.
    Delimiter = 7,
    //
    // Summary:
    //     A space, tab, or newline. Typically, a contiguous run of any whitespace is
    //     considered a single whitespace token. For example, the three spaces in "name
    //     this" would be treated as one whitespace token.
    WhiteSpace = 8,
    //
    // Summary:
    //     A line comment (comment is terminated at the end of the line). For example,
    //     in C# or C++, a comment is preceded by a //. In Visual Basic, this is a single
    //     tick '.
    LineComment = 9,
    //
    // Summary:
    //     A block comment. For example, in C# or C++, a comment is bounded by / * and
    //     * /. In XML, the comment is bounded by <!-- and -->.
    Comment = 10,
*/
%{
    #region IScanner Members

    public const int UNKNOWN_Char = -1;
	public const int KEYWORD_absolute = 2000;
	public const int KEYWORD_abstract = KEYWORD_absolute + 1;
	public const int KEYWORD_and = KEYWORD_abstract + 1;
	public const int KEYWORD_array = KEYWORD_and + 1;
	
	public const int KEYWORD_as = KEYWORD_array + 1;
	public const int KEYWORD_asm = KEYWORD_as + 1;
	public const int KEYWORD_begin = KEYWORD_asm + 1;
	public const int KEYWORD_case = KEYWORD_begin + 1;
	public const int KEYWORD_cdecl = KEYWORD_case + 1;
	public const int KEYWORD_class = KEYWORD_cdecl + 1;
	public const int KEYWORD_const = KEYWORD_class + 1;
	public const int KEYWORD_constructor = KEYWORD_const + 1;
	public const int KEYWORD_contains = KEYWORD_constructor + 1;
	public const int KEYWORD_default = KEYWORD_contains + 1;
	public const int KEYWORD_depricated = KEYWORD_default + 1;
	public const int KEYWORD_destructor = KEYWORD_depricated + 1;
	public const int KEYWORD_dispid = KEYWORD_destructor + 1;
	public const int KEYWORD_div = KEYWORD_dispid + 1;
	public const int KEYWORD_do = KEYWORD_div + 1;
	public const int KEYWORD_downto = KEYWORD_do + 1;
	public const int KEYWORD_dynamic = KEYWORD_downto + 1;
	public const int KEYWORD_else = KEYWORD_dynamic + 1;
	public const int KEYWORD_end = KEYWORD_else + 1;
	public const int KEYWORD_except = KEYWORD_end + 1;
	public const int KEYWORD_export = KEYWORD_except + 1;
	public const int KEYWORD_exports = KEYWORD_export + 1;
	public const int KEYWORD_external = KEYWORD_exports + 1;
	public const int KEYWORD_far = KEYWORD_external + 1;
	public const int KEYWORD_file = KEYWORD_far + 1;
	public const int KEYWORD_finalization = KEYWORD_file + 1;
	public const int KEYWORD_finally = KEYWORD_finalization + 1;
	public const int KEYWORD_for = KEYWORD_finally + 1;
	public const int KEYWORD_forward = KEYWORD_for + 1;
	public const int KEYWORD_function = KEYWORD_forward + 1;
	public const int KEYWORD_goto = KEYWORD_function + 1;
	public const int KEYWORD_if = KEYWORD_goto + 1;
	public const int KEYWORD_implementation = KEYWORD_if + 1;
	public const int KEYWORD_implements = KEYWORD_implementation + 1;
	public const int KEYWORD_in = KEYWORD_implements + 1;
	public const int KEYWORD_index = KEYWORD_in + 1;
	public const int KEYWORD_inherited = KEYWORD_index + 1;
	public const int KEYWORD_initialization = KEYWORD_inherited + 1;
	public const int KEYWORD_interface = KEYWORD_initialization + 1;
	public const int KEYWORD_is = KEYWORD_interface + 1;
	public const int KEYWORD_label = KEYWORD_is + 1;
	public const int KEYWORD_library = KEYWORD_label + 1;
	public const int KEYWORD_local = KEYWORD_library + 1;
	public const int KEYWORD_message = KEYWORD_local + 1;
	public const int KEYWORD_mod = KEYWORD_message + 1;
	public const int KEYWORD_name = KEYWORD_mod + 1;
	public const int KEYWORD_near = KEYWORD_name + 1;
	public const int KEYWORD_nil = KEYWORD_near + 1;
	public const int KEYWORD_nodefault = KEYWORD_nil + 1;
	public const int KEYWORD_not = KEYWORD_nodefault + 1;
	public const int KEYWORD_object = KEYWORD_not + 1;
	public const int KEYWORD_of = KEYWORD_object + 1;
	public const int KEYWORD_on = KEYWORD_of + 1;
	public const int KEYWORD_or = KEYWORD_on + 1;
	public const int KEYWORD_out = KEYWORD_or + 1;
	public const int KEYWORD_overload = KEYWORD_out + 1;
	public const int KEYWORD_override = KEYWORD_overload + 1;
	public const int KEYWORD_package = KEYWORD_override + 1;
	public const int KEYWORD_pascal = KEYWORD_package + 1;
	public const int KEYWORD_platform = KEYWORD_pascal + 1;
	public const int KEYWORD_private = KEYWORD_platform + 1;
	public const int KEYWORD_procedure = KEYWORD_private + 1;
	public const int KEYWORD_program = KEYWORD_procedure + 1;
	public const int KEYWORD_property = KEYWORD_program + 1;
	public const int KEYWORD_protected = KEYWORD_property + 1;
	public const int KEYWORD_public = KEYWORD_protected + 1;
	public const int KEYWORD_published = KEYWORD_public + 1;
	public const int KEYWORD_read = KEYWORD_published + 1;
	public const int KEYWORD_record = KEYWORD_read + 1;
	public const int KEYWORD_register = KEYWORD_record + 1;
	public const int KEYWORD_reintroduce = KEYWORD_register + 1;
	public const int KEYWORD_repeat = KEYWORD_reintroduce + 1;
	public const int KEYWORD_requires = KEYWORD_repeat + 1;
	public const int KEYWORD_resourcestring = KEYWORD_requires + 1;
	public const int KEYWORD_safecall = KEYWORD_resourcestring + 1;
	public const int KEYWORD_set = KEYWORD_safecall + 1;
	public const int KEYWORD_shl = KEYWORD_set + 1;
	public const int KEYWORD_shr = KEYWORD_shl + 1;
	public const int KEYWORD_stdcall = KEYWORD_shr + 1;
	public const int KEYWORD_stored = KEYWORD_stdcall + 1;
	public const int KEYWORD_string = KEYWORD_stored + 1;
	public const int KEYWORD_then = KEYWORD_string + 1;
	public const int KEYWORD_to = KEYWORD_then + 1;
	public const int KEYWORD_try = KEYWORD_to + 1;
	public const int KEYWORD_type = KEYWORD_try + 1;
	public const int KEYWORD_unit = KEYWORD_type + 1;
	public const int KEYWORD_until = KEYWORD_unit + 1;
	public const int KEYWORD_uses = KEYWORD_until + 1;
	public const int KEYWORD_var = KEYWORD_uses + 1;
	public const int KEYWORD_varargs = KEYWORD_var + 1;
	public const int KEYWORD_virtual = KEYWORD_varargs + 1;
	public const int KEYWORD_while = KEYWORD_virtual + 1;
	public const int KEYWORD_with = KEYWORD_while + 1;
	public const int KEYWORD_write = KEYWORD_with + 1;
	public const int KEYWORD_xor = KEYWORD_write + 1;
	public const int ITEM_Comment = KEYWORD_xor + 1;
	public const int COMPILER_Directive = ITEM_Comment + 1;
	public const int COMMENT_EndofLine = COMPILER_Directive + 1;
	public const int COMMENT_CurlyBrace = COMMENT_EndofLine + 1;  
	public const int COMMENT_BraceStar = COMMENT_CurlyBrace + 1;
    public const int IDENTIFIER = COMMENT_BraceStar + 1;
    public const int DIGIT = IDENTIFIER + 1;
    public const int HEX_Number = DIGIT + 1;
    public const int CHAR_HexValue = HEX_Number + 1;
    public const int CHAR_Value = CHAR_HexValue + 1;
    public const int FLOAT = CHAR_Value + 1;
    public const int REAL = FLOAT + 1;
    public const int STRING = REAL + 1;
    public const int ASSIGNMENT = STRING + 1;
    public const int DOES_NotEqual = ASSIGNMENT + 1;
    public const int LESS_ThanOrEqual = DOES_NotEqual + 1;
    public const int GREATER_ThanOrEqual = LESS_ThanOrEqual + 1;
    public const int OPEN_Brace = GREATER_ThanOrEqual + 1;
    public const int CLOSE_Brace = OPEN_Brace + 1;
    public const int OPEN_SquareBrace = CLOSE_Brace + 1;
    public const int CLOSE_SquareBrace = OPEN_SquareBrace + 1;
    public const int PERIOD = CLOSE_SquareBrace + 1;
    public const int OPERATORS = PERIOD + 1;
    public const int DELIMITERS = OPERATORS + 1;
    public const int WHITE_Space = DELIMITERS + 1;
    public const int END_Statement = WHITE_Space + 1;
	 
	
    private TokenInfo FTokenInfo;

    public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
    {
      if (zzEOFDone) return false;
      FLexicalState = state;
      FTokenInfo = tokenInfo;
      GetNextToken();
      state = this.yystate();
      return FTokenInfo.Token != -1;
    }

    public void SetSource(string source, int offset)
    {
      int lBackupState = FLexicalState;	
      zzEOFDone = false;							 
      yyreset(new StringReader(source.Substring(offset)));
      FLexicalState = lBackupState;
    }

    #endregion
    
    #region My Methods
    
  	private TokenInfo GetTokenInfo(int aAction, TokenType aTokenType)
    {
      FTokenInfo.Type = aTokenType;
      FTokenInfo.Token = aAction;
      FTokenInfo.StartIndex = FCurrentPos;
      FTokenInfo.EndIndex = FMarkedPos-1;
      switch (aTokenType) 
      {
        case TokenType.LineComment: 
        case TokenType.Comment: FTokenInfo.Color = TokenColor.Comment; break;
        case TokenType.Keyword: 
        case TokenType.Operator: 
        case TokenType.Delimiter: FTokenInfo.Color = TokenColor.Keyword; break;
        case TokenType.Literal: FTokenInfo.Color = TokenColor.Number; break;
        case TokenType.String: FTokenInfo.Color = TokenColor.String; break;
        case TokenType.Identifier: 
        case TokenType.Text: 
        case TokenType.WhiteSpace: FTokenInfo.Color = TokenColor.Text; break;
      }
      return FTokenInfo;
    }
   
    #endregion
    public DelphiScanner() : this(new StringReader(""))
    {
    }
  int FActualStart = 0;
/*  enum DelphiKeywordState { None = 0, Property = 1, Method = 2, Parameter = 4 };
  enum DelphiGoal { None, Program, Package, Library, Unit };
  DelphiKeywordState FCurrentState = DelphiKeywordState.None;
  DelphiGoal FDelphiGoal = DelphiGoal.None; */
%}  
/*	''	'test''s and all good Test''s'
List of regExp chars
. ^ $ * + ? { [ ] \ | ( )
		   #$AA #$ZZ #$1f' #99 #199 #399 #299 #259'
 {$ do not matach }
 {% do not match }									\#\$[0-9a-fA-F][0-9a-fA-F]|\#[0-9][0-9]|\#1[0-9][0-9]|\#2[0-5][0-9]
 { %$ match %$}
 {
   match
 }
*/
/* 
   '     = Find quote  
   [^']* = find any number of chars that is not quote
   ('')* = find any number of single quote displayed twice
   *     = repeat the process until you find only
   '     = one quote at the end
*/   


/*
TraditionalComment   = "{"[^$%]~"}"|"{""*"+"/"
CommentContent       = ( [^*] | \*+ [^/ *] )*
DocumentationComment = "/ **" {CommentContent} "*"+ "/" *)
*/
/* definitions of Items in the language */ 

LINE_Terminator		= \r|\n|\r\n
INPUT_Character		= [^\r\n]
WHITE_Space         = {LINE_Terminator} | [ \t\f]+
COMPILER_Directive  = "{$"[^\}]*"}"
ITEM_Comment        = "{%"[^\}]*"}"

END_OfLineComment   = "//"{INPUT_Character}*
START_Comment1	 	=  \{[^\$%]
END_Comment1        =  "}"
COMMENT_Text1	    = [^\}]*
START_Comment2	    =  "(*"
END_Comment2		=  "*)"
COMMENT_Text2	    = ([^*]|\*+[^)*])*
HEX_Digit			= [0-9a-fA-F]

IDENTIFIER 			= [a-zA-Z_]([a-zA-Z0-9_])*
DIGIT				= [0-9]
HEX_Number			= "$"{HEX_Digit}+
CHAR_HexValue		= \#{HEX_Number}
CHAR_Value			= \#{DIGIT}+
FLOAT			    = {DIGIT}+.{DIGIT}+
REAL				= {DIGIT}+(\.{DIGIT}*|{DIGIT}*)(E\+|E\-|E){DIGIT}+
STRING		 		= (({CHAR_Value}|{CHAR_HexValue})*('([^']*('')*)*')({CHAR_Value}|{CHAR_HexValue})*)+
ASSIGNMENT			= ":="
/*  @, not, ^, *, /, +, -, =, >, <, <>, <=, >=, or, xor, div, mod, and, shl, shr, as, in, is */
DOES_NotEqual       = "<>"
LESS_ThanOrEqual    = "<="
GREATER_ThanOrEqual = ">="
OPEN_Brace		= \(
CLOSE_Brace		= \)
OPEN_SquareBrace		= \[
CLOSE_SquareBrace		= \]
PERIOD				= \.
OPERATORS			= "*"|"/"|"^"|"+"|"-"|"="|">"|"<"|"@" 
END_Statement		= ";"
DELIMITERS			= ":"|","



/* keywords */
KEYWORD_absolute		      = absolute
KEYWORD_abstract              = abstract
KEYWORD_and                   = and
KEYWORD_array                 = array
KEYWORD_asm                   = asm
KEYWORD_as                    = as
KEYWORD_begin                 = begin
KEYWORD_case                  = case
KEYWORD_cdecl                 = cdecl
KEYWORD_class                 = class
KEYWORD_const                 = const
KEYWORD_constructor           = constructor
KEYWORD_contains              = contains
KEYWORD_default               = default
KEYWORD_depricated            = depricated
KEYWORD_destructor            = destructor
KEYWORD_dispid				  = dispid
KEYWORD_div                   = div
KEYWORD_do                    = do
KEYWORD_downto                = downto
KEYWORD_dynamic               = dynamic
KEYWORD_else                  = else
KEYWORD_end                   = end
KEYWORD_except                = except
KEYWORD_export                = export
KEYWORD_exports               = exports
KEYWORD_external              = external
KEYWORD_far                   = far
KEYWORD_file                  = file
KEYWORD_finalization          = finalization
KEYWORD_finally               = finally
KEYWORD_for                   = for
KEYWORD_forward               = forward
KEYWORD_function              = function
KEYWORD_goto                  = goto
KEYWORD_if                    = if
KEYWORD_implementation        = implementation
KEYWORD_implements            = implements
KEYWORD_index                 = index
KEYWORD_inherited             = inherited
KEYWORD_initialization        = initialization
KEYWORD_interface             = interface
KEYWORD_in					  =	in
KEYWORD_is                    = is
KEYWORD_label                 = label
KEYWORD_library               = library
KEYWORD_local                 = local
KEYWORD_message               = message
KEYWORD_mod                   = mod
KEYWORD_name                  = name
KEYWORD_near                  = near
KEYWORD_nil                   = nil
KEYWORD_nodefault             = nodefault
KEYWORD_not                   = not
KEYWORD_object                = object
KEYWORD_of                    = of
KEYWORD_on                    = on
KEYWORD_or                    = or
KEYWORD_out                   = out
KEYWORD_overload              = overload
KEYWORD_override              = override
KEYWORD_package               = package
KEYWORD_pascal                = pascal
KEYWORD_platform              = platform
KEYWORD_private               = private
KEYWORD_procedure             = procedure
KEYWORD_program               = program
KEYWORD_property              = property
KEYWORD_protected             = protected
KEYWORD_public                = public
KEYWORD_published             = published
KEYWORD_read                  = read
KEYWORD_record                = record
KEYWORD_register              = register
KEYWORD_reintroduce           = reintroduce
KEYWORD_repeat                = repeat
KEYWORD_requires              = requires
KEYWORD_resourcestring        = resourcestring
KEYWORD_safecall              = safecall
KEYWORD_set                   = set
KEYWORD_shl                   = shl
KEYWORD_shr                   = shr
KEYWORD_stdcall               = stdcall
KEYWORD_stored                = stored
KEYWORD_string                = string
KEYWORD_then                  = then
KEYWORD_to                    = to
KEYWORD_try                   = try
KEYWORD_type                  = type
KEYWORD_unit                  = unit
KEYWORD_until                 = until
KEYWORD_uses                  = uses
KEYWORD_var                   = var
KEYWORD_varargs               = varargs
KEYWORD_virtual               = virtual
KEYWORD_while                 = while
KEYWORD_with                  = with
KEYWORD_write                 = write
KEYWORD_xor                   = xor
/* end Keypwords */





       
%% 
{ITEM_Comment} { return GetTokenInfo(ITEM_Comment,TokenType.LineComment); }
{COMPILER_Directive}  {  return GetTokenInfo(COMPILER_Directive,TokenType.LineComment); }
{END_OfLineComment} {  return GetTokenInfo(COMMENT_EndofLine,TokenType.LineComment); }
{START_Comment1} { 
                   yybegin(COMMENT1); 
                   FActualStart = FCurrentPos; 
                 }
<COMMENT1>   
{
  {END_Comment1}    {  yybegin(YYINITIAL); FCurrentPos = FActualStart; return GetTokenInfo(COMMENT_CurlyBrace,TokenType.Comment); }
  {LINE_Terminator} { /* get more */ }
  {COMMENT_Text1}	  { /*  Text Data get more */ }
  <<EOF>>           { FCurrentPos = FActualStart;
                      TokenInfo lResult = GetTokenInfo(COMMENT_CurlyBrace,TokenType.Comment); 
                      FActualStart = 0; //You are passing in one line at a time so next comment start point will be zero
                      return lResult;
                    }
}
{START_Comment2} { 
                    yybegin(COMMENT2); 
                    FActualStart = FCurrentPos; 
                 }

<COMMENT2>   
{
  {END_Comment2}    { yybegin(YYINITIAL); 
                      FCurrentPos = FActualStart; // What point does the comment start  
                      return GetTokenInfo(COMMENT_BraceStar,TokenType.Comment); 
                    }
  {LINE_Terminator} { /* get more */ }
  {COMMENT_Text2}*	{ /* Text Data get more */ }
  <<EOF>>           { FCurrentPos = FActualStart;
                      TokenInfo lResult = GetTokenInfo(COMMENT_BraceStar,TokenType.Comment); 
                      FActualStart = 0; //You are passing in one line at a time so next comment start point will be zero
                      return lResult;
                    }
}

{KEYWORD_absolute}   { return GetTokenInfo(KEYWORD_absolute,TokenType.Keyword); }
{KEYWORD_abstract}           { return GetTokenInfo(KEYWORD_abstract,TokenType.Keyword); }
{KEYWORD_and}                { return GetTokenInfo(KEYWORD_and,TokenType.Keyword); }
{KEYWORD_array}      { return GetTokenInfo(KEYWORD_array,TokenType.Keyword); }
{KEYWORD_asm}   { return GetTokenInfo(KEYWORD_asm,TokenType.Keyword); }
{KEYWORD_as}   { return GetTokenInfo(KEYWORD_as,TokenType.Keyword); }
{KEYWORD_begin}   { return GetTokenInfo(KEYWORD_begin,TokenType.Keyword); }
{KEYWORD_case}   { return GetTokenInfo(KEYWORD_case,TokenType.Keyword); }
{KEYWORD_cdecl}   { return GetTokenInfo(KEYWORD_cdecl,TokenType.Keyword); }
{KEYWORD_class}   { return GetTokenInfo(KEYWORD_class,TokenType.Keyword); }
{KEYWORD_const}   { return GetTokenInfo(KEYWORD_const,TokenType.Keyword); }
{KEYWORD_constructor}   { return GetTokenInfo(KEYWORD_constructor,TokenType.Keyword); }
{KEYWORD_contains}   { return GetTokenInfo(KEYWORD_contains,TokenType.Keyword); }
{KEYWORD_default}   { return GetTokenInfo(KEYWORD_default,TokenType.Keyword); }
{KEYWORD_depricated}   { return GetTokenInfo(KEYWORD_depricated,TokenType.Keyword); }
{KEYWORD_destructor}   { return GetTokenInfo(KEYWORD_destructor,TokenType.Keyword); }
{KEYWORD_dispid}   { return GetTokenInfo(KEYWORD_dispid,TokenType.Keyword); }
{KEYWORD_div}   { return GetTokenInfo(KEYWORD_div,TokenType.Keyword); }
{KEYWORD_do}   { return GetTokenInfo(KEYWORD_do,TokenType.Keyword); }
{KEYWORD_downto}   { return GetTokenInfo(KEYWORD_downto,TokenType.Keyword); }
{KEYWORD_dynamic}   { return GetTokenInfo(KEYWORD_dynamic,TokenType.Keyword); }
{KEYWORD_else}   { return GetTokenInfo(KEYWORD_else,TokenType.Keyword); }
{KEYWORD_end}   { return GetTokenInfo(KEYWORD_end,TokenType.Keyword); }
{KEYWORD_except}   { return GetTokenInfo(KEYWORD_except,TokenType.Keyword); }
{KEYWORD_export}   { return GetTokenInfo(KEYWORD_export,TokenType.Keyword); }
{KEYWORD_exports}   { return GetTokenInfo(KEYWORD_exports,TokenType.Keyword); }
{KEYWORD_external}   { return GetTokenInfo(KEYWORD_external,TokenType.Keyword); }
{KEYWORD_far}   { return GetTokenInfo(KEYWORD_far,TokenType.Keyword); }
{KEYWORD_file}   { return GetTokenInfo(KEYWORD_file,TokenType.Keyword); }
{KEYWORD_finalization}   { return GetTokenInfo(KEYWORD_finalization,TokenType.Keyword); }
{KEYWORD_finally}   { return GetTokenInfo(KEYWORD_finally,TokenType.Keyword); }
{KEYWORD_for}   { return GetTokenInfo(KEYWORD_for,TokenType.Keyword); }
{KEYWORD_forward}   { return GetTokenInfo(KEYWORD_forward,TokenType.Keyword); }
{KEYWORD_function}   { return GetTokenInfo(KEYWORD_function,TokenType.Keyword); }
{KEYWORD_goto}   { return GetTokenInfo(KEYWORD_goto,TokenType.Keyword); }
{KEYWORD_if}   { return GetTokenInfo(KEYWORD_if,TokenType.Keyword); }
{KEYWORD_implementation}   { return GetTokenInfo(KEYWORD_implementation,TokenType.Keyword); }
{KEYWORD_implements}   { return GetTokenInfo(KEYWORD_implements,TokenType.Keyword); }
{KEYWORD_index}   { return GetTokenInfo(KEYWORD_index,TokenType.Keyword); }
{KEYWORD_inherited}   { return GetTokenInfo(KEYWORD_inherited,TokenType.Keyword); }
{KEYWORD_initialization}   { return GetTokenInfo(KEYWORD_initialization,TokenType.Keyword); }
{KEYWORD_interface}   { return GetTokenInfo(KEYWORD_interface,TokenType.Keyword); }
{KEYWORD_in}    { return GetTokenInfo(KEYWORD_in,TokenType.Keyword); }
{KEYWORD_is}    { return GetTokenInfo(KEYWORD_is,TokenType.Keyword); }
{KEYWORD_label}   { return GetTokenInfo(KEYWORD_label,TokenType.Keyword); }
{KEYWORD_library}   { return GetTokenInfo(KEYWORD_library,TokenType.Keyword); }
{KEYWORD_local}   { return GetTokenInfo(KEYWORD_local,TokenType.Keyword); }
{KEYWORD_message}   { return GetTokenInfo(KEYWORD_message,TokenType.Keyword); }
{KEYWORD_mod}   { return GetTokenInfo(KEYWORD_mod,TokenType.Keyword); }
{KEYWORD_name}   { return GetTokenInfo(KEYWORD_name,TokenType.Keyword); }
{KEYWORD_near}   { return GetTokenInfo(KEYWORD_near,TokenType.Keyword); }
{KEYWORD_nil}   { return GetTokenInfo(KEYWORD_nil,TokenType.Keyword); }
{KEYWORD_nodefault}   { return GetTokenInfo(KEYWORD_nodefault,TokenType.Keyword); }
{KEYWORD_not}   { return GetTokenInfo(KEYWORD_not,TokenType.Keyword); }
{KEYWORD_object}   { return GetTokenInfo(KEYWORD_object,TokenType.Keyword); }
{KEYWORD_of}   { return GetTokenInfo(KEYWORD_of,TokenType.Keyword); }
{KEYWORD_on}   { return GetTokenInfo(KEYWORD_on,TokenType.Keyword); }
{KEYWORD_or}   { return GetTokenInfo(KEYWORD_or,TokenType.Keyword); }
{KEYWORD_out}   { return GetTokenInfo(KEYWORD_out,TokenType.Keyword); }
{KEYWORD_overload}   { return GetTokenInfo(KEYWORD_overload,TokenType.Keyword); }
{KEYWORD_override}   { return GetTokenInfo(KEYWORD_override,TokenType.Keyword); }
{KEYWORD_package}   { return GetTokenInfo(KEYWORD_package,TokenType.Keyword); }
{KEYWORD_pascal}   { return GetTokenInfo(KEYWORD_pascal,TokenType.Keyword); }
{KEYWORD_platform}   { return GetTokenInfo(KEYWORD_platform,TokenType.Keyword); }
{KEYWORD_private}   { return GetTokenInfo(KEYWORD_private,TokenType.Keyword); }
{KEYWORD_procedure}   { return GetTokenInfo(KEYWORD_procedure,TokenType.Keyword); }
{KEYWORD_program}   { return GetTokenInfo(KEYWORD_program,TokenType.Keyword); }
{KEYWORD_property}   { return GetTokenInfo(KEYWORD_property,TokenType.Keyword); }
{KEYWORD_protected}   { return GetTokenInfo(KEYWORD_protected,TokenType.Keyword); }
{KEYWORD_public}   { return GetTokenInfo(KEYWORD_public,TokenType.Keyword); }
{KEYWORD_published}   { return GetTokenInfo(KEYWORD_published,TokenType.Keyword); }
{KEYWORD_read}   { return GetTokenInfo(KEYWORD_read,TokenType.Keyword); }
{KEYWORD_record}   { return GetTokenInfo(KEYWORD_record,TokenType.Keyword); }
{KEYWORD_register}   { return GetTokenInfo(KEYWORD_register,TokenType.Keyword); }
{KEYWORD_reintroduce}   { return GetTokenInfo(KEYWORD_reintroduce,TokenType.Keyword); }
{KEYWORD_repeat}   { return GetTokenInfo(KEYWORD_repeat,TokenType.Keyword); }
{KEYWORD_requires}   { return GetTokenInfo(KEYWORD_requires,TokenType.Keyword); }
{KEYWORD_resourcestring}   { return GetTokenInfo(KEYWORD_resourcestring,TokenType.Keyword); }
{KEYWORD_safecall}   { return GetTokenInfo(KEYWORD_safecall,TokenType.Keyword); }
{KEYWORD_set}   { return GetTokenInfo(KEYWORD_set,TokenType.Keyword); }
{KEYWORD_shl}   { return GetTokenInfo(KEYWORD_shl,TokenType.Keyword); }
{KEYWORD_shr}   { return GetTokenInfo(KEYWORD_shr,TokenType.Keyword); }
{KEYWORD_stdcall}   { return GetTokenInfo(KEYWORD_stdcall,TokenType.Keyword); }
{KEYWORD_stored}   { return GetTokenInfo(KEYWORD_stored,TokenType.Keyword); }
{KEYWORD_string}   { return GetTokenInfo(KEYWORD_string,TokenType.Keyword); }
{KEYWORD_then}   { return GetTokenInfo(KEYWORD_then,TokenType.Keyword); }
{KEYWORD_to}   { return GetTokenInfo(KEYWORD_to,TokenType.Keyword); }
{KEYWORD_try}   { return GetTokenInfo(KEYWORD_try,TokenType.Keyword); }
{KEYWORD_type}   { return GetTokenInfo(KEYWORD_type,TokenType.Keyword); }
{KEYWORD_unit}   { return GetTokenInfo(KEYWORD_unit,TokenType.Keyword); }
{KEYWORD_until}   { return GetTokenInfo(KEYWORD_until,TokenType.Keyword); }
{KEYWORD_uses}   { return GetTokenInfo(KEYWORD_uses,TokenType.Keyword); }
{KEYWORD_var}   { return GetTokenInfo(KEYWORD_var,TokenType.Keyword); }
{KEYWORD_varargs}   { return GetTokenInfo(KEYWORD_varargs,TokenType.Keyword); }
{KEYWORD_virtual}   { return GetTokenInfo(KEYWORD_virtual,TokenType.Keyword); }
{KEYWORD_while}   { return GetTokenInfo(KEYWORD_while,TokenType.Keyword); }
{KEYWORD_with}   { return GetTokenInfo(KEYWORD_with,TokenType.Keyword); }
{KEYWORD_write}   { return GetTokenInfo(KEYWORD_write,TokenType.Keyword); }
{KEYWORD_xor}   { return GetTokenInfo(KEYWORD_xor,TokenType.Keyword); }


{IDENTIFIER}   { return GetTokenInfo(IDENTIFIER,TokenType.Identifier); }
{DIGIT}   { return GetTokenInfo(DIGIT,TokenType.Literal); }
{HEX_Number}   { return GetTokenInfo(HEX_Number,TokenType.Literal); }
{CHAR_HexValue}   { return GetTokenInfo(CHAR_HexValue,TokenType.Literal); }
{CHAR_Value}   { return GetTokenInfo(CHAR_Value,TokenType.Literal); }
{FLOAT}   { return GetTokenInfo(FLOAT,TokenType.Literal); }
{REAL}   { return GetTokenInfo(REAL,TokenType.Literal); }
{STRING}   { return GetTokenInfo(STRING,TokenType.String); }
{ASSIGNMENT}   { return GetTokenInfo(ASSIGNMENT,TokenType.Operator); }
{DOES_NotEqual}   { return GetTokenInfo(DOES_NotEqual,TokenType.Operator); }
{LESS_ThanOrEqual}   { return GetTokenInfo(LESS_ThanOrEqual,TokenType.Operator); }
{GREATER_ThanOrEqual}   { return GetTokenInfo(GREATER_ThanOrEqual,TokenType.Operator); }
{OPEN_Brace}   { return GetTokenInfo(OPEN_Brace,TokenType.Delimiter); }
{CLOSE_Brace}   { return GetTokenInfo(CLOSE_Brace,TokenType.Delimiter); }
{OPEN_SquareBrace}   { return GetTokenInfo(OPEN_SquareBrace,TokenType.Delimiter); }
{CLOSE_SquareBrace}   { return GetTokenInfo(CLOSE_SquareBrace,TokenType.Delimiter); }
{PERIOD}   { return GetTokenInfo(PERIOD,TokenType.Delimiter); }
{OPERATORS}   { return GetTokenInfo(OPERATORS,TokenType.Operator); }
{END_Statement} { return GetTokenInfo(END_Statement,TokenType.Delimiter); }
{DELIMITERS}   { return GetTokenInfo(DELIMITERS,TokenType.Delimiter); }
{WHITE_Space}   { return GetTokenInfo(WHITE_Space,TokenType.WhiteSpace); }


.|[^.] { return GetTokenInfo(UNKNOWN_Char,TokenType.Unknown); }


/*{UNKNOWN_Char} { return GetTokenInfo(UNKNOWN_Char,TokenType.Unknown); }*/
<<EOF>> { return GetTokenInfo(-1,TokenType.Unknown); }

%%

} // end namespace
