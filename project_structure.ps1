# Nur src, tests und Tools Ordner + Root-Dateien
Get-ChildItem -Path @(".\src", ".\tests", ".\Tools", ".\*.md", ".\*.props", ".\*.sln", ".\*.json") -Recurse -Include *.cs,*.csproj,*.xaml,*.json,*.md,*.props,*.sln,*.exe -File -ErrorAction SilentlyContinue | 
    Where-Object { $_.FullName -notmatch "\\(bin|obj|\.vs|packages|TestResults)\\|\.Designer\.cs$" } | 
    ForEach-Object { $_.FullName.Replace((Get-Location).Path + "\", "") } | 
    Sort-Object | 
    Out-File -FilePath "project_structure.txt" -Encoding UTF8

# Dann anzeigen
Write-Host "Struktur gespeichert in project_structure.txt"
Get-Content "project_structure.txt"