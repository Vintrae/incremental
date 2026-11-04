$ErrorActionPreference = "Stop"

$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$godotConsole = $env:GODOT_CONSOLE

if (-not $godotConsole) {
    $godotCommand = Get-Command "Godot_v*_mono_win64_console.exe" -ErrorAction SilentlyContinue |
        Select-Object -First 1
    if ($godotCommand) {
        $godotConsole = $godotCommand.Source
    }
}

if (-not $godotConsole) {
    $godotParent = Split-Path $projectRoot -Parent
    $godotConsole = Get-ChildItem -Path $godotParent -Filter "Godot_v*_mono_win64_console.exe" -File -ErrorAction SilentlyContinue |
        Sort-Object Name -Descending |
        Select-Object -First 1 -ExpandProperty FullName
}

if (-not $godotConsole -or -not (Test-Path $godotConsole -PathType Leaf)) {
    throw "Could not find the Godot .NET console executable. Set GODOT_CONSOLE to its full path."
}

& $godotConsole --headless --path $projectRoot "res://Tests/Godot/FishingRadiusIntegrationTest.tscn"
exit $LASTEXITCODE
