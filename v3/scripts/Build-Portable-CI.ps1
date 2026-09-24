# Build Anbarban v3 — برنامهٔ جدا + پیش‌نیازهای جدا (آفلاین)
$ErrorActionPreference = "Stop"
$v3Root = Split-Path -Parent $PSScriptRoot
$repoRoot = Split-Path -Parent $v3Root
Set-Location $v3Root

Write-Host "== dotnet build Release x64 =="
dotnet build Anbarban.sln -c Release
if ($LASTEXITCODE -ne 0) { throw "dotnet build failed" }

$src = Join-Path $v3Root "Anbarban.Wpf\bin\Release\net48"
if (-not (Test-Path (Join-Path $src "Anbarban.exe"))) { throw "Anbarban.exe not found in $src" }

$releaseDir = Join-Path $repoRoot "release"
$appName = "Anbarban-v3-App"
$appDir = Join-Path $releaseDir $appName
$appZip = Join-Path $releaseDir "$appName.zip"

if (Test-Path $appDir) { Remove-Item -Recurse -Force $appDir }
New-Item -ItemType Directory -Path (Join-Path $appDir "Data") | Out-Null
New-Item -ItemType Directory -Path (Join-Path $appDir "tools") | Out-Null
New-Item -ItemType Directory -Path (Join-Path $appDir "prerequisites") | Out-Null

Copy-Item -Path "$src\*" -Destination $appDir -Recurse -Force
# redist/install-ace داخل اپ نمی‌رود — پیش‌نیاز فقط از ZIP جدا

# حذف redist از خروجی build اگر کپی شده
$redistInApp = Join-Path $appDir "redist"
if (Test-Path $redistInApp) { Remove-Item -Recurse -Force $redistInApp }

$dbSrc = Join-Path $repoRoot "database\Inventory.accdb"
if (Test-Path $dbSrc) {
  Copy-Item $dbSrc (Join-Path $appDir "Data\Inventory.accdb")
} else {
  @"
اگر Inventory.accdb اینجا نیست:
- بعد از نصب ACE، Anbarban.exe را بزنید (معمولاً خودکار ساخته می‌شود)
- یا «ساخت-پایگاه-داده.bat» را اجرا کنید
"@ | Set-Content -Path (Join-Path $appDir "Data\README.txt") -Encoding UTF8
}

Copy-Item (Join-Path $repoRoot "release\راهنما-انباربان.txt") (Join-Path $appDir "راهنما.txt") -ErrorAction SilentlyContinue
Copy-Item (Join-Path $v3Root "راهنما-نسخه۳.txt") (Join-Path $appDir "راهنما-نسخه۳.txt") -ErrorAction SilentlyContinue
Copy-Item (Join-Path $repoRoot "tools\generate_unlock_code.py") (Join-Path $appDir "tools\") -ErrorAction SilentlyContinue
Copy-Item (Join-Path $repoRoot "tools\مولد-کد-مدیر.bat") (Join-Path $appDir "tools\") -ErrorAction SilentlyContinue

$buildDb = Join-Path $appDir "build-db"
New-Item -ItemType Directory -Path $buildDb | Out-Null
Copy-Item (Join-Path $repoRoot "build\Build-Database-TablesOnly.vbs") $buildDb -Force
Copy-Item (Join-Path $v3Root "portable-template\ساخت-پایگاه-داده.bat") (Join-Path $appDir "ساخت-پایگاه-داده.bat") -Force

Copy-Item (Join-Path $v3Root "prerequisites-template\شروع-بخوانید-پیش‌نیاز.txt") (Join-Path $appDir "prerequisites\اینجا-پیش‌نیاز-بریزید.txt") -ErrorAction SilentlyContinue

@"
انباربان v3 — برنامه (آفلاین)

این ZIP فقط خود برنامه است.

۱) از Release هم «Anbarban-v3-Prerequisites.zip» را بگیرید.
۲) محتوای ZIP پیش‌نیاز را در پوشه prerequisites کنار Anbarban.exe بریزید.
۳) روی PC انبار: نصب-پیش‌نیازها.bat (در prerequisites) سپس «شروع انباربان.bat».

بکاپ: Data\Inventory.accdb
"@ | Set-Content -Path (Join-Path $appDir "شروع-بخوانید.txt") -Encoding UTF8

@"
@echo off
chcp 65001 >nul
cd /d "%~dp0"
start "" "Anbarban.exe"
"@ | Set-Content -Path (Join-Path $appDir "شروع انباربان.bat") -Encoding ASCII

if (Test-Path $appZip) { Remove-Item -Force $appZip }
Compress-Archive -Path $appDir -DestinationPath $appZip -Force
Write-Host "OK: $appZip"

Write-Host "== prerequisites package =="
& (Join-Path $PSScriptRoot "Build-Prerequisites-CI.ps1")

# سازگاری با نام قبلی — همان محتوای App
$legacyZip = Join-Path $releaseDir "Anbarban-v3-Portable.zip"
Copy-Item $appZip $legacyZip -Force
Write-Host "OK (legacy name): $legacyZip"
