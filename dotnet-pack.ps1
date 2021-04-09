[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$signingKeyPath
)

dotnet pack Independer.WCFDataAnnotations\Independer.WCFDataAnnotations.csproj `
        /p:SignAssembly=true `
        /p:AssemblyOriginatorKeyFile=$signingKeyPath `
        /p:Configuration=Release `
        --output "publish\" `
        --include-symbols `
        --verbosity Detailed 