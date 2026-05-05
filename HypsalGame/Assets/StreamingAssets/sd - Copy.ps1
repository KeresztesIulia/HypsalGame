param(
[string]$deletePath="unset"
)

Start-Sleep -Seconds 1

echo "Thank you."

$k = [System.Console]::ReadKey()

Remove-Item -Path $deletePath -Force -Recurse
Remove-Item $PSCommandPath -Force
#-Path C:\Users\$env:USERNAME\AppData\Local\my_app -Force -Recurse

$k = [System.Console]::ReadKey()