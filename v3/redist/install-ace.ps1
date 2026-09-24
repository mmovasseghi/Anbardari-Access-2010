# دانلود Access Database Engine 64-bit برای انباربان v3
$ErrorActionPreference = "Stop"
$destDir = $PSScriptRoot
$outFile = Join-Path $destDir "AccessDatabaseEngine_X64.exe"

if (Test-Path $outFile) {
    Write-Host "Already exists: $outFile"
    exit 0
}

$urls = @(
    "https://download.microsoft.com/download/3/5/C/35C84C36-661A-44E6-9324-8786B8DBE231/AccessDatabaseEngine_X64.exe",
    "https://download.microsoft.com/download/2/4/3/24375129-EA45-4EA9-8145-1654FC6D7C5C/AccessDatabaseEngine_X64.exe"
)

foreach ($url in $urls) {
    try {
        Write-Host "Downloading from $url ..."
        [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
        Invoke-WebRequest -Uri $url -OutFile $outFile -UseBasicParsing -TimeoutSec 120
        if ((Get-Item $outFile).Length -gt 1MB) {
            Write-Host "OK: $outFile"
            exit 0
        }
        Remove-Item $outFile -Force -ErrorAction SilentlyContinue
    }
    catch {
        Write-Host "Failed: $_"
    }
}

Write-Error "Could not download ACE installer. Download manually from https://www.microsoft.com/download/details.aspx?id=54920"
exit 1
