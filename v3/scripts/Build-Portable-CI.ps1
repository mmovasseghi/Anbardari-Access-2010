# Build Anbarban v3 portable ZIP (Windows CI or local)
$ErrorActionPreference = "Stop"
$v3Root = Split-Path -Parent $PSScriptRoot
$repoRoot = Split-Path -Parent $v3Root
Set-Location $v3Root

Write-Host "== dotnet build Release x64 =="
dotnet build Anbarban.sln -c Release
if ($LASTEXITCODE -ne 0) { throw "dotnet build failed" }

$src = Join-Path $v3Root "Anbarban.Wpf\bin\Release\net48"
if (-not (Test-Path (Join-Path $src "Anbarban.exe"))) { throw "Anbarban.exe not found in $src" }

$outName = "Anbarban-v3-Portable"
$releaseDir = Join-Path $repoRoot "release"
$outDir = Join-Path $releaseDir $outName
$zipPath = Join-Path $releaseDir "$outName.zip"

if (Test-Path $outDir) { Remove-Item -Recurse -Force $outDir }
New-Item -ItemType Directory -Path (Join-Path $outDir "Data") | Out-Null
New-Item -ItemType Directory -Path (Join-Path $outDir "tools") | Out-Null

Copy-Item -Path "$src\*" -Destination $outDir -Recurse -Force

$dbSrc = Join-Path $repoRoot "database\Inventory.accdb"
if (Test-Path $dbSrc) {
  Copy-Item $dbSrc (Join-Path $outDir "Data\Inventory.accdb")
} else {
  @"
اگر Inventory.accdb اینجا نیست:
- اول Anbarban.exe را بزنید (با ACE 64-bit معمولاً خودکار ساخته می‌شود)
- یا «ساخت-پایگاه-داده.bat» را اجرا کنید
"@ | Set-Content -Path (Join-Path $outDir "Data\README.txt") -Encoding UTF8
}

Copy-Item (Join-Path $repoRoot "release\راهنما-انباربان.txt") (Join-Path $outDir "راهنما.txt") -ErrorAction SilentlyContinue
Copy-Item (Join-Path $v3Root "راهنما-نسخه۳.txt") (Join-Path $outDir "راهنما-نسخه۳.txt") -ErrorAction SilentlyContinue
Copy-Item (Join-Path $repoRoot "tools\generate_unlock_code.py") (Join-Path $outDir "tools\") -ErrorAction SilentlyContinue
Copy-Item (Join-Path $repoRoot "tools\مولد-کد-مدیر.bat") (Join-Path $outDir "tools\") -ErrorAction SilentlyContinue

# ساخت پایگاه — فقط جداول (بدون frmMain)
$buildDb = Join-Path $outDir "build-db"
New-Item -ItemType Directory -Path $buildDb | Out-Null
Copy-Item (Join-Path $repoRoot "build\Build-Database-TablesOnly.vbs") $buildDb -Force
Copy-Item (Join-Path $v3Root "portable-template\ساخت-پایگاه-داده.bat") (Join-Path $outDir "ساخت-پایگاه-داده.bat") -Force

@"
انباربان v3 — پرتابل

۱) روی «شروع انباربان.bat» دوبار کلیک کنید.

۲) اگر پیام «پایگاه داده پیدا نشد» دیدید:
   - «ساخت-پایگاه-داده.bat» (فقط جداول، بدون frmMain)
   - یا Anbarban.exe را بزنید (خودکار اگر ACE نصب باشد)
   - فایل Inventory.accdb را در پوشه Data کپی کنید.

۳) پیش‌نیاز: .NET 4.8 + ACE OLEDB 64-bit

بکاپ: Data\Inventory.accdb
"@ | Set-Content -Path (Join-Path $outDir "شروع-بخوانید.txt") -Encoding UTF8

@"
@echo off
chcp 65001 >nul
cd /d "%~dp0"
start "" "Anbarban.exe"
"@ | Set-Content -Path (Join-Path $outDir "شروع انباربان.bat") -Encoding ASCII

if (Test-Path $zipPath) { Remove-Item -Force $zipPath }
Compress-Archive -Path $outDir -DestinationPath $zipPath -Force

Write-Host "OK: $zipPath"
Get-Item $zipPath | Format-List Name, Length
