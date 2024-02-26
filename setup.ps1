$eschoolbotpath = 'C:\Program Files\eschoolbot'

mkdir $eschoolbotpath

$acl = Get-Acl -Path $eschoolbotpath
$ace = New-Object System.Security.Accesscontrol.FileSystemAccessRule ($env:USERNAME, "Write", "Allow")
$acl.AddAccessRule($ace)
Set-Acl -Path $eschoolbotpath -AclObject $acl

Set-Location $eschoolbotpath

git clone https://github.com/rinrab/eschoolbot repos

dotnet publish .\repos\ESchoolBot\ESchoolBot.csproj --output .\app\

New-EventLog -LogName eschoolbot -Source eschoolbot

Set-Content -Path "Update.xml" -Value '<?xml version="1.0" encoding="UTF-16"?>
<Task version="1.2" xmlns="http://schemas.microsoft.com/windows/2004/02/mit/task">
  <Triggers>
    <BootTrigger>
      <Enabled>true</Enabled>
    </BootTrigger>
    <RegistrationTrigger>
      <Enabled>true</Enabled>
    </RegistrationTrigger>
  </Triggers>
  <Principals>
    <Principal id="Author">
      <UserId>S-1-5-18</UserId>
      <RunLevel>HighestAvailable</RunLevel>
    </Principal>
  </Principals>
  <Settings>
    <MultipleInstancesPolicy>IgnoreNew</MultipleInstancesPolicy>
    <DisallowStartIfOnBatteries>true</DisallowStartIfOnBatteries>
    <StopIfGoingOnBatteries>true</StopIfGoingOnBatteries>
    <AllowHardTerminate>true</AllowHardTerminate>
    <StartWhenAvailable>false</StartWhenAvailable>
    <RunOnlyIfNetworkAvailable>false</RunOnlyIfNetworkAvailable>
    <IdleSettings>
      <StopOnIdleEnd>true</StopOnIdleEnd>
      <RestartOnIdle>false</RestartOnIdle>
    </IdleSettings>
    <AllowStartOnDemand>true</AllowStartOnDemand>
    <Enabled>true</Enabled>
    <Hidden>false</Hidden>
    <RunOnlyIfIdle>false</RunOnlyIfIdle>
    <WakeToRun>false</WakeToRun>
    <ExecutionTimeLimit>PT72H</ExecutionTimeLimit>
    <Priority>7</Priority>
  </Settings>
  <Actions Context="Author">
    <Exec>
      <Command>powershell.exe</Command>
      <Arguments>-File "$eschoolbotpath\Update.ps1"</Arguments>
    </Exec>
  </Actions>
</Task>'

Set-Content -Path "Update.ps1" -Value "cd $PSScriptRoot\repos
git pull
Stop-Service eschoolbot
dotnet publish .\ESchoolBot\ESchoolBot.csproj --output ..\app\
Start-Service eschoolbot"

schtasks.exe /create /tn eschool_update /xml "$PSScriptRoot\eschool_update.xml"
