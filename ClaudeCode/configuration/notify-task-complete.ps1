# Send Windows Toast Notification
param(
    [string]$Title = "Task Complete",
    
    [string]$Message = "Your task has been completed successfully!",
    
    [string]$AppId = "Claude Code Task Complete"
)

# Load the Windows Runtime to access toast notifications
[Windows.UI.Notifications.ToastNotificationManager, Windows.UI.Notifications, ContentType = WindowsRuntime] | Out-Null
[Windows.Data.Xml.Dom.XmlDocument, Windows.Data.Xml.Dom.XmlDocument, ContentType = WindowsRuntime] | Out-Null

# Create the toast notification XML template
$toastXml = @"
<toast>
    <visual>
        <binding template="ToastText02">
            <text>$Title</text>
            <text>$Message</text>
        </binding>
    </visual>
</toast>
"@

# Create XML document and load the template
$xmlDoc = New-Object Windows.Data.Xml.Dom.XmlDocument
$xmlDoc.LoadXml($toastXml)

# Create and show the toast notification
$toast = [Windows.UI.Notifications.ToastNotification]::new($xmlDoc)
$notifier = [Windows.UI.Notifications.ToastNotificationManager]::CreateToastNotifier($AppId)
$notifier.Show($toast)

Write-Host "Notification sent: $Title - $Message"