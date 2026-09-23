@echo off
chcp 65001 >nul
cd /d "%~dp0"
cscript //nologo Build-Database-TablesOnly.vbs "%~dp0..\database\Inventory.accdb"
pause
