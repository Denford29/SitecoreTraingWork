@echo off
REM load settings
@call %0\..\settings.cmd

REM serialize
xcopy %tools%\serialization.aspx %wwwroot%\%1\Website\sitecore\admin /Y /Q
call %tools%\curl\curl -F "dump2=master;select=master;type=application/x-www-form-urlencoded" http://%1/sitecore/admin/serialization.aspx > nul 2>&1
IF EXIST %serializationfolder% GOTO COPYFILES
mkdir %serializationfolder%
:COPYFILES
xcopy %wwwroot%\%1\data\serialization %serializationfolder% /S /Y /Q
