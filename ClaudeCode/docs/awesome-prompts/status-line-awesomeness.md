# Some cool things you can do with status line and sessions

1. Empowered by https://www.youtube.com/watch?v=oWsjmNSxoLQ

## Log details of your session on a new statusline.log file

1. Prompt -> Add a --debug flag to @statusline.sh that logs the input JSON to .claude/statusline.log with timestamps.

## Show what you typed in claude console in status line

1. Prompt -> Add a second line showing the last user prompt. Extract from the transcript file at ~/.claude/projects/{encoded-path}/{session_id}.json, find the last user message, display with -> symbol, and truncate to 100 characters with ... if longer.

## Want claude to summarize your last session?

1. Look at statuslog.json file that it created on first step above
2. Then do  `claude -p "Summarize the following transcript... C:\\Users\\261906\\.claude\\projects\\C--Projects-ROME-rome-repairorder-service\\52740707-1fc3-4daf-a03c-9e6bfef591c9.jsonl"`
