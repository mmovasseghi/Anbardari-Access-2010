@echo off
chcp 65001 >nul
title انباربان — ساخت دیتابیس
cd /d "%~dp0"
if not exist "build\ساخت-دیتابیس.bat" (
  echo پوشه build پیدا نشد. ZIP را کامل Extract کنید.
  pause
  exit /b 1
)
cd build
call "ساخت-دیتابیس.bat"
