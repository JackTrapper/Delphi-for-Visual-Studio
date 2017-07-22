program DCC32V15_0Test;

 {$IFDEF TestDelphiIncludePath}
  {$I 'IncludeFile1.INC'}
 {$ENDIF}
uses
 {$IFNDEF CONSOLEAPP}
  Forms,
  Unit1 in 'Unit1.pas' {Form1};
 {$ENDIF}
 {$IFDEF CONSOLEAPP}
   System;
//	Unit2, // in "Test Path1"
//	Unit3; // in "Test Path2"
 {$ENDIF}
 
 {$R *.res}

 {$IFDEF CONSOLEAPP}
 var
	 lLine1 :String;
	 lLine2 :String;
 {$ENDIF}
begin
 {$IFNDEF CONSOLEAPP}
  Application.Initialize;
  Application.CreateForm(TForm1, Form1);
  Application.Run;
 {$ENDIF}

 {$IFDEF CONSOLEAPP}
	lLine1 := 'Console app test';
	lLine2 := 'Hello world.';
  {$IFDEF TestDelphiIncludePath}
	  {$IFNDEF IncludeFile1}
	   Error include file did not import
	  {$ELSE}
	   Line2 := 'Include file worked';
	  {$ENDIF}
  {$ENDIF}
 {$ENDIF}
 
 
 {$IFDEF CONSOLEAPP}
  WriteLn(lLine1);
  WriteLn(lLine2);
 {$ENDIF}

end.
