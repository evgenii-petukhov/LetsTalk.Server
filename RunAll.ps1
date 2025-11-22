Write-Host "Starting Docker Desktop..."
Start-Process -FilePath "C:\Program Files\Docker\Docker\Docker Desktop.exe"

Write-Host "Waiting for Docker engine to be ready..."
while (-not (docker info 2>$null)) {
    Write-Host "." -NoNewline
    Start-Sleep -Seconds 1
}
Write-Host "`nDocker engine is ready."

Write-Host "Waiting briefly for Docker pipe to initialize..."
Start-Sleep -Seconds 3
Write-Host "Proceeding with container startup."

# Function to start a Docker container and wait for its 'running' status (using the fixed function from previous answer)
function Start-DockerContainerAndWait {
    # ... (function code as provided in the previous answer, using status 'running'/'exited') ...
    param([string]$ContainerName)

    Write-Host "`nStarting $ContainerName..."
    # Ensure this line uses your current working directory path if running from an IDE
    docker start $ContainerName | Out-Null 

    Write-Host "Waiting for $ContainerName to be running..."
    
    $containerRunning = $false
    $containerExited = $false

    while (-not $containerRunning -and -not $containerExited) {
        Start-Sleep -Seconds 1
        $state = docker container inspect -f '{{.State.Status}}' $ContainerName 2>$null

        if ($state -eq "running") {
            $containerRunning = $true
        } elseif ($state -eq "exited") {
            $containerExited = $true
        }
        Write-Host "." -NoNewline
    }
    Write-Host "`n"

    if ($containerRunning) {
        Write-Host "$ContainerName is running."
    } elseif ($containerExited) {
        Write-Host "$ContainerName exited unexpectedly. Please check 'docker logs $ContainerName'."
        # You might want to 'exit 1' here to stop the whole script
    }
}

# Start containers
Start-DockerContainerAndWait -ContainerName redis
Start-DockerContainerAndWait -ContainerName kafka

Write-Host "`nClean-up binaries..."
Get-ChildItem -include bin,obj -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }

Write-Host "`nBuilding..."
dotnet publish -r win-x64 --self-contained false --configuration Debug

Write-Host "`Launching..."
dotnet publish -r win-x64 --self-contained false --configuration Debug
Start-Process -FilePath "LetsTalk.Server.API\bin\Debug\net9.0\win-x64\publish\LetsTalk.Server.API.exe"
Start-Process -FilePath "LetsTalk.Server.Authentication\bin\Debug\net9.0\win-x64\publish\LetsTalk.Server.Authentication.exe"
Start-Process -FilePath "LetsTalk.Server.FileStorage.Service\bin\Debug\net9.0\win-x64\publish\LetsTalk.Server.FileStorage.Service.exe"
Start-Process -FilePath "LetsTalk.Server.ImageProcessing.Service\bin\Debug\net9.0\win-x64\publish\LetsTalk.Server.ImageProcessing.Service.exe"
Start-Process -FilePath "LetsTalk.Server.LinkPreview\bin\Debug\net9.0\win-x64\publish\LetsTalk.Server.LinkPreview.exe"
Start-Process -FilePath "LetsTalk.Server.Notifications\bin\Debug\net9.0\win-x64\publish\LetsTalk.Server.Notifications.exe"
Start-Process -FilePath "LetsTalk.Server.EmailService\bin\Debug\net9.0\win-x64\publish\LetsTalk.Server.EmailService.exe"
