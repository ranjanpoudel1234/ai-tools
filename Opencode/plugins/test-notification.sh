#!/usr/bin/env bash

# Test script for Windows notification plugin
# This script tests the PowerShell notification command directly

echo "Testing Windows notification from WSL..."
echo ""

# Test 1: Toast notification
echo "Test 1: Sending toast notification..."
powershell.exe -NoProfile -Command "
  [Windows.UI.Notifications.ToastNotificationManager, Windows.UI.Notifications, ContentType = WindowsRuntime] > \$null
  \$template = [Windows.UI.Notifications.ToastNotificationManager]::GetTemplateContent([Windows.UI.Notifications.ToastTemplateType]::ToastText02)
  
  \$toastXml = [xml] \$template.GetXml()
  \$toastXml.GetElementsByTagName('text')[0].AppendChild(\$toastXml.CreateTextNode('OpenCode Test')) > \$null
  \$toastXml.GetElementsByTagName('text')[1].AppendChild(\$toastXml.CreateTextNode('This is a test notification from OpenCode')) > \$null
  
  \$xml = New-Object Windows.Data.Xml.Dom.XmlDocument
  \$xml.LoadXml(\$toastXml.OuterXml)
  \$toast = [Windows.UI.Notifications.ToastNotification]::new(\$xml)
  \$toast.Tag = 'OpenCode'
  \$toast.Group = 'OpenCode'
  
  \$notifier = [Windows.UI.Notifications.ToastNotificationManager]::CreateToastNotifier('OpenCode')
  \$notifier.Show(\$toast)
"

if [ $? -eq 0 ]; then
  echo "✓ Toast notification sent successfully!"
else
  echo "✗ Toast notification failed"
fi

echo ""
echo "Test 2: Playing notification sound..."
powershell.exe -NoProfile -Command "[System.Media.SystemSounds]::Asterisk.Play()"

if [ $? -eq 0 ]; then
  echo "✓ Sound played successfully!"
else
  echo "✗ Sound failed"
fi

echo ""
echo "If you saw a notification and heard a sound, the plugin will work correctly!"
