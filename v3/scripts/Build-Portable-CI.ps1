# Build Anbarban v3 — پرتابل + پیش‌نیازها (دو ZIP، بدون تکرار App/Portable)
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
$portableName = "Anbarban-v3-Portable"
$outDir = Join-Path $releaseDir $portableName
$portableZip = Join-Path $releaseDir "$portableName.zip"
$obsoleteAppZip = Join-Path $releaseDir "Anbarban-v3-App.zip"

if (Test-Path $outDir) { Remove-Item -Recurse -Force $outDir }
New-Item -ItemType Directory -Path (Join-Path $outDir "Data") | Out-Null
New-Item -ItemType Directory -Path (Join-Path $outDir "tools") | Out-Null
New-Item -ItemType Directory -Path (Join-Path $outDir "prerequisites") | Out-Null

Copy-Item -Path "$src\*" -Destination $outDir -Recurse -Force

$redistInApp = Join-Path $outDir "redist"
if (Test-Path $redistInApp) { Remove-Item -Recurse -Force $redistInApp }

$dbSrc = Join-Path $repoRoot "database\Inventory.accdb"
if (Test-Path $dbSrc) {
  Copy-Item $dbSrc (Join-Path $outDir "Data\Inventory.accdb")
} else {
  @"
اگر Inventory.accdb اینجا نیست:
- بعد از نصب ACE، Anbarban.exe را بزنید (معمولاً خودکار ساخته می‌شود)
- یا «ساخت-پایگاه-داده.bat» را اجرا کنید
"@ | Set-Content -Path (Join-Path $outDir "Data\README.txt") -Encoding UTF8
}

Copy-Item (Join-Path $repoRoot "release\راهنما-انباربان.txt") (Join-Path $outDir "راهنما.txt") -ErrorAction SilentlyContinue
Copy-Item (Join-Path $v3Root "راهنما-نسخه۳.txt") (Join-Path $outDir "راهنما-نسخه۳.txt") -ErrorAction SilentlyContinue
Copy-Item (Join-Path $repoRoot "tools\generate_unlock_code.py") (Join-Path $outDir "tools\") -ErrorAction SilentlyContinue
Copy-Item (Join-Path $repoRoot "tools\مولد-کد-مدیر.bat") (Join-Path $outDir "tools\") -ErrorAction SilentlyContinue

$buildDb = Join-Path $outDir "build-db"
New-Item -ItemType Directory -Path $buildDb | Out-Null
Copy-Item (Join-Path $repoRoot "build\Build-Database-TablesOnly.vbs") $buildDb -Force
Copy-Item (Join-Path $v3Root "portable-template\ساخت-پایگاه-داده.bat") (Join-Path $outDir "ساخت-پایگاه-داده.bat") -Force

Copy-Item (Join-Path $v3Root "prerequisites-template\شروع-بخوانید-پیش‌نیاز.txt") (Join-Path $outDir "prerequisites\اینجا-پیش‌نیاز-بریزید.txt") -ErrorAction SilentlyContinue

@"
انباربان v3 — پرتابل (آفلاین)

این ZIP خود برنامه است (نصب نمی‌خواهد؛ Extract و «شروع انباربان.bat»).

۱) از Release هم «Anbarban-v3-Prerequisites.zip» را بگیرید.
۲) محتوای ZIP پیش‌نیاز را در پوشه prerequisites کنار Anbarban.exe بریزید.
۳) prerequisites\نصب-پیش‌نیازها.bat سپس «شروع انباربان.bat».

بکاپ: Data\Inventory.accdb
"@ | Set-Content -Path (Join-Path $outDir "شروع-بخوانید.txt") -Encoding UTF8

@"
@echo off
chcp 65001 >nul
cd /d "%~dp0"
start "" "Anbarban.exe"
"@ | Set-Content -Path (Join-Path $outDir "شروع انباربان.bat") -Encoding ASCII

if (Test-Path $portableZip) { Remove-Item -Force $portableZip }
if (Test-Path $obsoleteAppZip) { Remove-Item -Force $obsoleteAppZip }
Compress-Archive -Path $outDir -DestinationPath $portableZip -Force
Write-Host "OK: $portableZip"

Write-Host "== prerequisites package =="
& (Join-Path $PSScriptRoot "Build-Prerequisites-CI.ps1")

# پوشهٔ موقت App (اگر از نسخهٔ قبل مانده)
$obsoleteAppDir = Join-Path $releaseDir "Anbarban-v3-App"
if (Test-Path $obsoleteAppDir) { Remove-Item -Recurse -Force $obsoleteAppDir }
