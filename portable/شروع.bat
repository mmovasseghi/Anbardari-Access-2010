@echo off
chcp 65001 >nul
cd /d "%~dp0"
start "" mshta.exe "%~dp0app\Anbardari.hta"
