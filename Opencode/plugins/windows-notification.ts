import type { Plugin } from "@opencode-ai/plugin"

/**
 * Windows Notification Plugin
 * 
 * Sends Windows toast notifications when OpenCode needs user input.
 * Works in WSL by using PowerShell to trigger Windows notifications.
 * 
 * Triggers notifications on:
 * - session.idle: When OpenCode finishes processing and is waiting
 * - permission.asked: When OpenCode needs permission approval
 * - session.error: When an error occurs during processing
 */
export const WindowsNotificationPlugin: Plugin = async ({ project, client, $ }) => {
  // Function to send Windows notification via PowerShell
  const sendNotification = async (title: string, message: string, sound: boolean = true) => {
    try {
      // Escape single quotes in title and message
      const escapedTitle = title.replace(/'/g, "''")
      const escapedMessage = message.replace(/'/g, "''")
      
      // PowerShell script to show Windows 10/11 toast notification
      const psScript = `
        [Windows.UI.Notifications.ToastNotificationManager, Windows.UI.Notifications, ContentType = WindowsRuntime] > $null
        $template = [Windows.UI.Notifications.ToastNotificationManager]::GetTemplateContent([Windows.UI.Notifications.ToastTemplateType]::ToastText02)
        
        $toastXml = [xml] $template.GetXml()
        $toastXml.GetElementsByTagName("text")[0].AppendChild($toastXml.CreateTextNode('${escapedTitle}')) > $null
        $toastXml.GetElementsByTagName("text")[1].AppendChild($toastXml.CreateTextNode('${escapedMessage}')) > $null
        
        $xml = New-Object Windows.Data.Xml.Dom.XmlDocument
        $xml.LoadXml($toastXml.OuterXml)
        $toast = [Windows.UI.Notifications.ToastNotification]::new($xml)
        $toast.Tag = "OpenCode"
        $toast.Group = "OpenCode"
        
        $notifier = [Windows.UI.Notifications.ToastNotificationManager]::CreateToastNotifier("OpenCode")
        $notifier.Show($toast)
      `.trim()

      // Execute PowerShell command from WSL
      await $`powershell.exe -NoProfile -Command ${psScript}`.quiet()
      
      // Optional: Play a sound using Windows Media Player
      if (sound) {
        await $`powershell.exe -NoProfile -Command "[System.Media.SystemSounds]::Asterisk.Play()"`.quiet()
      }
    } catch (error) {
      // Fallback to simple notification if toast fails
      try {
        await $`powershell.exe -NoProfile -Command "Add-Type -AssemblyName System.Windows.Forms; [System.Windows.Forms.MessageBox]::Show('${message}', '${title}', 'OK', 'Information')"`.quiet()
      } catch (fallbackError) {
        await client.app.log({
          service: "windows-notification",
          level: "error",
          message: "Failed to send notification",
          extra: { error: String(error), fallbackError: String(fallbackError) },
        })
      }
    }
  }

  return {
    event: async ({ event }) => {
      // Notification when OpenCode goes idle (needs user input)
      if (event.type === "session.idle") {
        await sendNotification(
          "OpenCode - Input Needed",
          "OpenCode is waiting for your input",
          false
        )
        
        await client.app.log({
          service: "windows-notification",
          level: "info",
          message: "Sent idle notification",
        })
      }

      // Notification when permission is needed
      if (event.type === "permission.asked") {
        await sendNotification(
          "OpenCode - Permission Required",
          "OpenCode needs your permission to continue",
          false
        )
        
        await client.app.log({
          service: "windows-notification",
          level: "info",
          message: "Sent permission notification",
        })
      }

      // Notification when an error occurs
      if (event.type === "session.error") {
        await sendNotification(
          "OpenCode - Error",
          "An error occurred. Please check the session.",
          false
        )
        
        await client.app.log({
          service: "windows-notification",
          level: "error",
          message: "Sent error notification",
        })
      }
    },
  }
}
