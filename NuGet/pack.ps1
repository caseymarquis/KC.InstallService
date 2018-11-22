$root = (split-path -parent $MyInvocation.MyCommand.Definition) + '\..'
$version = [System.Reflection.Assembly]::LoadFile("$root\KC.InstallService\bin\Release\KC.InstallService.dll").GetName().Version
$versionStr = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build)

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\NuGet\KC.InstallService.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $root\nuget\KC.InstallService.compiled.nuspec

& $root\NuGet\NuGet.exe pack $root\nuget\KC.InstallService.compiled.nuspec
