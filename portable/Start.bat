@echo off
chcp 65001 >nul
cd /d "%~dp0"

REM 1) Try HTA portable (no install)
if exist "%~dp0app\Anbardari.hta" (
  start "" mshta.exe "%~dp0app\Anbardari.hta"
  exit /b 0
)

REM 2) Fallback Python if available
where py >nul 2>&1 && (
  start "" py -3 "%~dp0app\anbardari.py"
  exit /b 0
)
where python >nul 2>&1 && (
  start "" python "%~dp0app\anbardari.py"
  exit /b 0
)

echo No runtime found.
pause
