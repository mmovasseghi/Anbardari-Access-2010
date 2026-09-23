@echo off
chcp 65001 >nul
cd /d "%~dp0"
echo.
echo ========================================
echo   ساخت خودکار دیتابیس انبارداری
echo   Microsoft Access 2010
echo ========================================
echo.
echo در حال ساخت... لطفاً صبر کنید.
echo.
cscript //nologo "Build-Inventory.vbs"
echo.
echo اگر Access باز شد، کار درست انجام شده است.
echo فایل نهایی: ..\database\Inventory.accdb
echo.
pause
