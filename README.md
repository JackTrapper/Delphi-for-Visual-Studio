Project Description
-------------------
Delphi Win32 development in Visual Studio


Why am I doing this?
--------------------

I have written a large amount of code in Delphi and it's IDE has too many issues. Also the Delphi IDE has features that I do not like or just do not work. If you feel the same way that I do then please help me with testing if you can. However if you do not feel the way I do, good, I am happy for you and I would suggest you continue using Delphi's IDE. 

Here are some reasons I am building this product.

1. My code mostly has COM and is not visual code.
2. I have seen many issues in the type library editor.
3. I use a source code control system and the .DOF, RES, DPR, CFG file change too often
4. I want code projects in one IDE and you like Visual Studio
5. I plan to migrate from Delphi Win32 to C#

What features will be supported in the first beta?

- MSBuild project management under solutions
- Binary files like .RES, .TLB files can be used but will not be required project files
- A compiled Delphi for Visual Studio project can be loaded in Delphi 7 IDE
- A text based resource file (.RC) can be used instead that will compile to .RES
- Project properties dialog to update the MSBuild project
- Type library files will be edited by the .IDL file and compiled to .TLB
- Syntax high lighting
- Project items : Unit, Type Library (IDL), and Text File
- Folder links to search paths
- File links to files outside of the Delphi project
- Project compilation and execution

Features like form design, intelli sense and debugging will not be in first beta.  
Last edited Nov 19, 2007 at 9:29 PM by [Davincij](https://www.codeplex.com/site/users/view/Davincij), version 10

Migrated from [CodePlex - Delphi for Visual Studio](https://delphi4visualstudio.codeplex.com/)
