@echo off
chcp 65001 >nul
cd /d "%~dp0"
echo Building Inventory.accdb for Access 2010 ...
cscript //nologo "Build-Inventory.vbs"
echo.
echo Output: ..\database\Inventory.accdb
pause
