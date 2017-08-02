@echo off
REM load settings
@call %0\..\settings.cmd

REM delete install
call %sim% delete -n %1
rmdir /S /Q %wwwroot%\%1
REM install Sitecore
call %sim% install -n %1
REM compile solution
pushd ..
%tools%\nuget\nuget.exe restore
call %msbuild%
popd
call %msbuild% /t:Package /p:_PackageTempDir=%packageoutput% /p:configuration="Debug"
REM copy solution files
xcopy %packageoutput% %wwwroot%\%1\Website /Y /S /Q
REM copy serialized items
IF EXIST %wwwroot%\%1\data\serialization GOTO COPYFILES
mkdir %wwwroot%\%1\data\serialization
:COPYFILES
xcopy %serializationfolder% %wwwroot%\%1\Data\serialization /Y /S /Q
REM deserialize
xcopy %tools%\serialization.aspx %wwwroot%\%1\Website\sitecore\admin /Y /Q
call %tools%\curl\curl -F "update=master;select=master;type=application/x-www-form-urlencoded" http://%1/sitecore/admin/serialization.aspx > nul 2>&1
