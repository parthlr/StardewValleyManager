param (
    [ValidateSet('win-x64', 'win-arm64')]
    [string] $Runtime = 'win-x64',
    [switch] $PublishAot = $false,
    [switch] $PublishSingleFile = $false
)

dotnet publish ./Desktop/Desktop.csproj `
     -r $Runtime `
     -o .\publish\$Runtime `
     -c Release  `
     -f net8.0 `
     -p:PublishAot=$PublishAot `
     -p:PublishReadyToRun=true  `
     -p:PublishTrimmed=true `
     -p:TrimMode=full `
     -p:IncludeNativeLibrariesForSelfExtract=true `
     -p:PublishSingleFile=$PublishSingleFile `
     --self-contained