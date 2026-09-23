@echo off
chcp 65001 >nul
title مولد کد مدیر — انباربان
cd /d "%~dp0"
set KIND=%~1
set DOCID=%~2
set DOCNO=%~3
if "%KIND%"=="" (
  echo استفاده:
  echo   مولد-کد-مدیر.bat INCOMING 5 "شماره-سند"
  echo   مولد-کد-مدیر.bat OUTGOING 3 "شماره-سند"
  pause
  exit /b 1
)
python generate_unlock_code.py %KIND% %DOCID% %DOCNO%
if errorlevel 1 py generate_unlock_code.py %KIND% %DOCID% %DOCNO%
pause
