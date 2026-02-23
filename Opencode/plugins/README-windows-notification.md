# Windows Notification Plugin

A plugin for OpenCode that sends Windows toast notifications when OpenCode needs your input.

## Features

- **Toast Notifications**: Shows modern Windows 10/11 toast notifications
- **Sound Alerts**: Plays system sounds with notifications
- **Multiple Triggers**: Notifies on different events
- **WSL Compatible**: Works seamlessly from WSL (Windows Subsystem for Linux)

## Installation

The plugin is automatically loaded from your `.opencode/plugins/` directory.

No additional configuration needed - just make sure the file exists:
```
~/.opencode/plugins/windows-notification.ts
```

## Events Monitored

The plugin sends notifications for these events:

### 1. `session.idle`
**When**: OpenCode finishes processing and is waiting for your input
**Notification**: "OpenCode - Input Needed"
**Message**: "OpenCode is waiting for your input"

### 2. `permission.asked`
**When**: OpenCode needs your permission to perform an action
**Notification**: "OpenCode - Permission Required"  
**Message**: "OpenCode needs your permission to continue"

### 3. `session.error`
**When**: An error occurs during processing
**Notification**: "OpenCode - Error"
**Message**: "An error occurred. Please check the session."

## Testing

Run the test script to verify notifications work:

```bash
cd ~/.opencode
./plugins/test-notification.sh
```

You should see:
- A Windows toast notification appear
- Hear a system sound
- Success messages in the terminal

## How It Works

The plugin uses PowerShell commands from WSL to trigger native Windows notifications:

1. **Toast Notifications**: Uses Windows.UI.Notifications API
2. **Sound**: Uses System.Media.SystemSounds
3. **Fallback**: Falls back to MessageBox if toast fails

## Customization

You can customize the plugin by editing `plugins/windows-notification.ts`:

### Change notification sound
```typescript
// In sendNotification function, change:
[System.Media.SystemSounds]::Asterisk.Play()

// To other sounds like:
[System.Media.SystemSounds]::Beep.Play()
[System.Media.SystemSounds]::Exclamation.Play()
[System.Media.SystemSounds]::Hand.Play()
[System.Media.SystemSounds]::Question.Play()
```

### Disable sound
```typescript
await sendNotification(
  "OpenCode - Input Needed",
  "OpenCode is waiting for your input",
  false  // Set to false to disable sound
)
```

### Add more events
```typescript
// Add in the event handler:
if (event.type === "command.executed") {
  await sendNotification(
    "OpenCode - Command Executed",
    "A command has finished executing"
  )
}
```

Available events:
- `command.executed`
- `file.edited`
- `session.created`
- `session.completed`
- `session.idle`
- `session.error`
- `permission.asked`
- `permission.replied`
- `todo.updated`

See [OpenCode Plugin Documentation](https://opencode.ai/docs/plugins) for full list.

## Requirements

- Windows 10 or Windows 11
- WSL (Windows Subsystem for Linux)
- PowerShell (comes with Windows)

## Troubleshooting

### Notifications don't appear
1. Check Windows notification settings (Settings → System → Notifications)
2. Ensure "Focus Assist" is not blocking notifications
3. Run the test script to debug

### Permission errors
Make sure the test script is executable:
```bash
chmod +x ~/.opencode/plugins/test-notification.sh
```

### Plugin not loading
Check OpenCode logs:
```bash
opencode --verbose
```

Look for messages from `windows-notification` service.

## Related Files

- `plugins/windows-notification.ts` - Main plugin file
- `plugins/test-notification.sh` - Test script
- `opencode.json` - OpenCode configuration

## License

This plugin is part of your OpenCode configuration and can be modified as needed.
