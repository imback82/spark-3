@echo off

setlocal enabledelayedexpansion

set SHELL_DIR=%~dp0

@REM Remove trailing backslash \
set SHELL_DIR=%SHELL_DIR:~0,-1%

if "%JAVA_HOME%" == "" goto :javahomeerror
if "%SPARK_HOME%" == "" goto :sparkhomeerror
if "%SPARK_MAJOR_MINOR_VERSION%" == "" goto :sparkmajorminofrversionerror
if "%DOTNET_SPARK_VERSION%" == "" goto :dotnetsparkversionerror
if "%DOTNET_SPARK_CONFIGURATION%" == "" goto :dotnetsparkconfigurationerror

%SPARK_HOME%\bin\spark-submit.cmd --master local --class org.apache.spark.deploy.dotnet.DotnetRunner %SHELL_DIR%\..\src\scala\microsoft-spark-%SPARK_MAJOR_MINOR_VERSION%.x\target\microsoft-spark-%SPARK_MAJOR_MINOR_VERSION%.x-%DOTNET_SPARK_VERSION%.jar dotnet %SHELL_DIR%\..\artifacts\bin\Microsoft.Spark.REPL\%DOTNET_SPARK_CONFIGURATION%\netcoreapp2.1\Microsoft.Spark.REPL.dll

goto :eof

:javahomeerror
	@echo [sparkdotnet-shell.cmd] Error - JAVA_HOME environment variable is not set
	goto :eof

:sparkhomeerror
	@echo [sparkdotnet-shell.cmd] Error - SPARK_HOME environment variable is not set
	goto :eof
	
:sparkmajorminofrversionerror
	@echo [sparkdotnet-shell.cmd] Error - SPARK_MAJOR_MINOR_VERSION environment variable is not set
	@echo [sparkdotnet-shell.cmd] It should be set to 2.3 or 2.4, for example.
	goto :eof

:dotnetsparkversionerror
	@echo [sparkdotnet-shell.cmd] Error - DOTNET_SPARK_VERSION environment variable is not set
	@echo [sparkdotnet-shell.cmd] It should be set to the version of Microsoft.Spark (0.5.0, for example).
	goto :eof

:dotnetsparkconfigurationerror
	@echo [sparkdotnet-shell.cmd] Error - DOTNET_SPARK_CONFIGURATION environment variable is not set
	@echo [sparkdotnet-shell.cmd] It should be set to the build configration of Microsoft.Spark.REPL.dll (Debug/Release).
	goto :eof