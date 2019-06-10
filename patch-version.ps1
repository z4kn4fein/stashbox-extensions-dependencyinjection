param($projectPath, $version)

Write-Host "Patching with version $version"

$csprojPath = Join-Path $PSScriptRoot $projectPath
[xml]$project = Get-Content -Path $csprojPath
$group = $project.Project.PropertyGroup;
if ($group.count -gt 1) {
    $group[0].Version = $version
} else {
    $group.Version = $version
}
$project.Save($csprojPath)