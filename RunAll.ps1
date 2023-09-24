Get-ChildItem -include bin,obj -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }
dotnet publish -r win-x64
start LetsTalk.Server.API\bin\Debug\net7.0\win-x64\publish\LetsTalk.Server.API.exe
start LetsTalk.Server.Authentication\bin\Debug\net7.0\win-x64\publish\LetsTalk.Server.Authentication.exe
start LetsTalk.Server.FileStorage.Service\bin\Debug\net7.0\win-x64\publish\LetsTalk.Server.FileStorage.Service.exe
start LetsTalk.Server.ImageProcessing.Service\bin\Debug\net7.0\win-x64\publish\LetsTalk.Server.ImageProcessing.Service.exe
start LetsTalk.Server.LinkPreview\bin\Debug\net7.0\win-x64\publish\LetsTalk.Server.LinkPreview.exe
start LetsTalk.Server.Notifications\bin\Debug\net7.0\win-x64\publish\LetsTalk.Server.Notifications.exe