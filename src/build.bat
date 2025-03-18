@echo off
set _CONFIG=Release
set _TARGET=x64

call dotnet build Solid.sln /p:Platform=%_TARGET% /p:Configuration=%_CONFIG%

dir .\_Build\%_CONFIG% /b
rem dir .\_Build\%_CONFIG%\*.dll /s /b
dir .\_Build\%_CONFIG%\*.exe /s /b
