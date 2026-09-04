param(
    [string]$Destino = (Join-Path $env:USERPROFILE "Desktop\DevTools Share - Compartilhar")
)

$ErrorActionPreference = "Stop"
$projeto = Join-Path $PSScriptRoot "DTSWindowsForm\DTSWindowsForm\DTSWindowsForm.csproj"

Write-Host "Publicando DevTools Share em arquivo único: $Destino"
dotnet publish $projeto `
    -c Release `
    -r win-x64 `
    --self-contained true `
    --no-restore `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:DebugType=None `
    -p:DebugSymbols=false `
    -o $Destino

if ($LASTEXITCODE -ne 0) {
    throw "A publicação do DevTools Share falhou."
}

$executavel = Join-Path $Destino "DevToolsShare.exe"
if (-not (Test-Path -LiteralPath $executavel)) {
    throw "O executável não foi encontrado depois da publicação."
}

Write-Host "Pasta pronta para compartilhar: $Destino"
Write-Host "Executável: $executavel"
