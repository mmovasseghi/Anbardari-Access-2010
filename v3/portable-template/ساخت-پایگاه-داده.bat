@echo off
chcp 65001 >nul
title انباربان — ساخت پایگاه (فقط جداول)
cd /d "%~dp0"
set DB=%~dp0Data\Inventory.accdb
if not exist "%~dp0Data" mkdir "%~dp0Data"

if exist "%~dp0build-db\Build-Database-TablesOnly.vbs" (
  cscript //nologo "%~dp0build-db\Build-Database-TablesOnly.vbs" "%DB%"
  goto :done
)

echo فایل build-db پیدا نشد. Anbarban.exe را بزنید — اگر ACE نصب باشد خودش پایگاه می‌سازد.
pause
exit /b 1

:done
echo.
echo اگر موفق بود، Anbarban.exe را اجرا کنید.
pause
