@echo off
chcp 65001 >nul
setlocal
cd /d "%~dp0\.."
echo Building Release...
dotnet build Anbarban.sln -c Release
if errorlevel 1 (
  echo Build failed. Install .NET SDK + targeting pack net48 on this machine.
  pause
  exit /b 1
)

set OUT=..\release\Anbarban-v3-Portable
set SRC=Anbarban.Wpf\bin\Release\net48
rmdir /s /q "%OUT%" 2>nul
mkdir "%OUT%\Data"

xcopy /y /q "%SRC%\*.*" "%OUT%\"
if exist "..\..\database\Inventory.accdb" copy /y "..\..\database\Inventory.accdb" "%OUT%\Data\"
copy /y "..\release\راهنما-انباربان.txt" "%OUT%\راهنما.txt" 2>nul

echo.
echo Portable folder: %OUT%
pause
