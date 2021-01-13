@echo off
rem batch file for making a Coco/R C# application - CSC 301 2015
if "%1" == "" goto missing
if not exist %1.atg goto badfile
coco %1.atg -options m %2 %3 %4 %5 %6
if errorlevel 1 goto badatg
if not exist %1.cs goto stop
csc %1.cs Parser.cs Scanner.cs > errors
if errorlevel 1 goto errors
echo Compiled %1.exe
goto stop
:errors
echo C# errors - see file "errors"
goto stop
:missing
echo No grammar file specified - use cmake ATGFile [options]
goto stop
:badfile
echo Cannot find %1.atg
goto stop
:badatg
listing.txt
:stop

