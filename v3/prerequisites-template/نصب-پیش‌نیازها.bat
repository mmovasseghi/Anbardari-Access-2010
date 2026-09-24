@echo off
chcp 65001 >nul
cd /d "%~dp0"
echo.
echo === انباربان — نصب پیش‌نیازها (آفلاین) ===
echo.

set "DOTNET=ndp48-x86-x64-allos-enu.exe"
set "ACE=AccessDatabaseEngine_X64.exe"

if exist "%DOTNET%" (
  echo [1/2] نصب .NET Framework 4.8 ...
  start /wait "" "%DOTNET%" /q /norestart
) else (
  echo [1/2] فایل %DOTNET% اینجا نیست — اگر .NET 4.8 نصب است رد کنید.
)

if exist "%ACE%" (
  echo [2/2] نصب موتور پایگاه ACE 64-bit ...
  start /wait "" "%ACE%" /passive
) else (
  echo [2/2] خطا: %ACE% پیدا نشد. ZIP پیش‌نیازها را کامل کپی کنید.
  pause
  exit /b 1
)

echo.
echo تمام. حالا برنامه را از پوشهٔ بالا با «شروع انباربان.bat» باز کنید.
pause
