# Capture Anbarban v3 window screenshots for README (Windows PowerShell 5.1+)
$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.Windows.Forms, System.Drawing
Add-Type @"
using System;
using System.Drawing;
using System.Runtime.InteropServices;
public static class WinScreenCap {
  [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
  [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
  public struct RECT { public int Left, Top, Right, Bottom; }
  public static void Capture(IntPtr hWnd, string path) {
    RECT r; if (!GetWindowRect(hWnd, out r)) throw new Exception("GetWindowRect");
    int w = Math.Max(1, r.Right - r.Left), h = Math.Max(1, r.Bottom - r.Top);
    using (var bmp = new Bitmap(w, h)) {
      using (var g = Graphics.FromImage(bmp))
        g.CopyFromScreen(r.Left, r.Top, 0, 0, new Size(w, h));
      bmp.Save(path);
    }
  }
}
"@

$v3Root = Split-Path -Parent $PSScriptRoot
$repoRoot = Split-Path -Parent $v3Root
$exe = Join-Path $v3Root "Anbarban.Wpf\bin\Release\net48\Anbarban.exe"
$outDir = Join-Path $repoRoot "docs\screenshots\v3"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

if (-not (Test-Path $exe)) {
  dotnet build (Join-Path $v3Root "Anbarban.sln") -c Release | Out-Null
}

function Wait-MainWindow($proc, $sec = 15) {
  for ($i = 0; $i -lt $sec * 2; $i++) {
    $proc.Refresh()
    if ($proc.MainWindowHandle -ne [IntPtr]::Zero) { return $proc.MainWindowHandle }
    Start-Sleep -Milliseconds 500
  }
  throw "Main window not found"
}

function Snap($proc, $name) {
  $hwnd = Wait-MainWindow $proc
  [void][WinScreenCap]::SetForegroundWindow($hwnd)
  Start-Sleep -Milliseconds 600
  $path = Join-Path $outDir $name
  [WinScreenCap]::Capture($hwnd, $path)
  Write-Host "OK $path ($((Get-Item $path).Length) bytes)"
}

$proc = Start-Process $exe -PassThru
try {
  Snap $proc "01-home.png"
  # Additional pages would need UI automation; home is the hero shot for README.
}
finally {
  Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
}
