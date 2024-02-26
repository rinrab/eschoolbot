$eschoolbotpath = 'C:\Program Files\eschoolbot'

mkdir $eschoolbotpath

$acl = Get-Acl -Path $eschoolbotpath
$ace = New-Object System.Security.Accesscontrol.FileSystemAccessRule ($env:USERNAME, "Write", "Allow")
$acl.AddAccessRule($ace)
Set-Acl -Path $eschoolbotpath -AclObject $acl

cd 'C:\Program Files\eschoolbot'

git clone https://github.com/rinrab/eschoolbot repos

dotnet publish .\repos\ESchoolBot\ESchoolBot.csproj --output .\app\

New-EventLog -LogName eschoolbot -Source eschoolbot
