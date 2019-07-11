echo off  
ParserBases.exe %1  
If errorlevel -1 goto NoArg   
if errorlevel 0 echo Completed Successfully   
goto :EOF  

:NoArg  
echo Missing argument  
goto :EOF  