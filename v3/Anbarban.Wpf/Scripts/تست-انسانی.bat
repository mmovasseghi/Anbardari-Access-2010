@echo off
chcp 65001 >nul
cd /d "%~dp0..\bin\Debug\net48"
echo تست انسانی — MainWindow واقعی، FlaUI موس/کیبورد
Anbarban.exe --ui-human-test --ui-human-test-minutes=10
echo Exit: %ERRORLEVEL%
pause
