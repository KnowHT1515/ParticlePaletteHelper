param(
    [Parameter(Mandatory = $true)]
    [string]$CelesteDir
)

$ErrorActionPreference = "Stop"
$ProjectPath = Join-Path $PSScriptRoot "ParticlePaletteHelper.csproj"
$BuildDirectory = Join-Path $PSScriptRoot "build"
$IntermediateDirectory = Join-Path $PSScriptRoot "obj"
$MappedSourceRoot = "/_/ParticlePaletteHelper"

$requiredAssemblies = @("Celeste.dll", "MMHOOK_Celeste.dll", "FNA.dll")
foreach ($assembly in $requiredAssemblies) {
    if (-not (Test-Path (Join-Path $CelesteDir $assembly))) {
        throw "$assembly not found in '$CelesteDir'. Pass the active Everest Celeste directory."
    }
}

foreach ($directory in @($BuildDirectory, $IntermediateDirectory)) {
    if (Test-Path $directory) {
        Remove-Item $directory -Recurse -Force
    }
}

$buildArguments = @(
    "build",
    $ProjectPath,
    "--configuration", "Release",
    "--output", $BuildDirectory,
    "--no-incremental",
    "-p:CelesteDir=$CelesteDir",
    "-p:PathMap=$PSScriptRoot=$MappedSourceRoot"
)

& dotnet @buildArguments
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

$artifacts = @(
    (Join-Path $BuildDirectory "ParticlePaletteHelper.dll"),
    (Join-Path $BuildDirectory "ParticlePaletteHelper.pdb")
)

$localPathForms = @(
    $PSScriptRoot,
    $PSScriptRoot.Replace("\", "/")
)

foreach ($artifact in $artifacts) {
    if (-not (Test-Path $artifact)) {
        throw "Expected build artifact was not produced: $artifact"
    }

    $binaryText = [System.Text.Encoding]::UTF8.GetString([System.IO.File]::ReadAllBytes($artifact))
    foreach ($localPath in $localPathForms) {
        if ($binaryText.IndexOf($localPath, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) {
            throw "Local source path leaked into build artifact: $artifact"
        }
    }
}

Write-Host ""
Write-Host "Deterministic release build complete. Source paths are mapped to $MappedSourceRoot."
Write-Host "Copy these files into ParticlePaletteHelper/Code/:"
Write-Host "  $BuildDirectory\ParticlePaletteHelper.dll"
Write-Host "  $BuildDirectory\ParticlePaletteHelper.pdb  (optional; useful for diagnostics)"
