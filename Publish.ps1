cd $PSScriptRoot
git pull
schtasks.exe /end /tn ESchoolBot
dotnet publish ESchoolBot /p:PublishProfile=FolderProfile
schtasks.exe /run /tn ESchoolBot
