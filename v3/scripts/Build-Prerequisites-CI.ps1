# بستهٔ پیش‌نیاز آفلاین — دانلود فقط در CI (Release)، نه روی PC انبار
$ErrorActionPreference = "Stop"
$v3Root = Split-Path -Parent $PSScriptRoot
$repoRoot = Split-Path -Parent $v3Root
$releaseDir = Join-Path $repoRoot "release"
$outName = "Anbarban-v3-Prerequisites"
$outDir = Join-Path $releaseDir $outName
$zipPath = Join-Path $releaseDir "$outName.zip"

if (Test-Path $outDir) { Remove-Item -Recurse -Force $outDir }
New-Item -ItemType Directory -Path $outDir | Out-Null

$template = Join-Path $v3Root "prerequisites-template"
Copy-Item -Path "$template\*" -Destination $outDir -Recurse -Force

function Download-File($url, $dest) {
    Write-Host "Downloading $url ..."
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    Invoke-WebRequest -Uri $url -OutFile $dest -UseBasicParsing -TimeoutSec 300
}

$aceDest = Join-Path $outDir "AccessDatabaseEngine_X64.exe"
$aceUrls = @(
    "https://download.microsoft.com/download/3/5/C/35C84C36-661A-44E6-9324-8786B8DBE231/AccessDatabaseEngine_X64.exe",
    "https://download.microsoft.com/download/2/4/3/24375129-EA45-4EA9-8145-1654FC6D7C5C/AccessDatabaseEngine_X64.exe"
)
foreach ($u in $aceUrls) {
    try {
        Download-File $u $aceDest
        if ((Get-Item $aceDest).Length -gt 1MB) { break }
        Remove-Item $aceDest -Force -ErrorAction SilentlyContinue
    } catch { Write-Host "ACE URL failed: $_" }
}
if (-not (Test-Path $aceDest)) { throw "Could not download ACE installer for prerequisites package" }

$dotnetDest = Join-Path $outDir "ndp48-x86-x64-allos-enu.exe"
try {
    Download-File "https://go.microsoft.com/fwlink/?LinkId=2088631" $dotnetDest
} catch {
    Write-Host "Warning: .NET 4.8 offline installer download failed — package will ship ACE only."
    @"
فایل ndp48-x86-x64-allos-enu.exe در این بسته نیست.
روی یک PC با اینترنت از لینک مایکروسافت .NET Framework 4.8 Offline Installer را بگیرید
و در همین پوشه prerequisites کنار Anbarban.exe قرار دهید.
"@ | Set-Content -Path (Join-Path $outDir "راهنمای-دات‌نت-4.8.txt") -Encoding UTF8
}

if (Test-Path $zipPath) { Remove-Item -Force $zipPath }
Compress-Archive -Path $outDir -DestinationPath $zipPath -Force
Write-Host "OK: $zipPath"
Get-Item $zipPath | Format-List Name, Length
