param(
[string]$deletePath="unset"
)

Write-Host "Removing AI..."

Start-Sleep -Seconds 1


for ($i = 0; $i -lt 10; $i++) {
    try {
        Remove-Item $deletePath -Force -Recurse
        Write-Host "Thank you."
        break
    } catch {
        Write-Host "In progress..."
        Start-Sleep -Seconds 1
    }
}

$k = [System.Console]::ReadKey()

Remove-Item $PSCommandPath -Force