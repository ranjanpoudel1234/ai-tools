---
name: youtube-summarizer
description: Fetches the transcript of any YouTube video via yt-dlp and produces a rich, structured summary with key takeaways, section breakdown, and helpful links. AUTOMATICALLY INVOKE when user provides a YouTube URL and asks for a summary, transcript, or overview of the video.
---

## Overview

This skill downloads the auto-generated subtitle track from any YouTube video using **yt-dlp**, parses it into clean text, and produces a well-structured markdown summary with:

- High-level TL;DR
- Section-by-section breakdown of the content
- Key takeaways / actionable insights
- A links table for every resource mentioned in the video

## When to Use

Invoke this skill whenever the user:
- Shares a YouTube URL and asks for a "summary", "overview", "transcript", "recap", or "what is this about"
- Wants to extract key points or learn from a video without watching it
- Wants to catch up on a video and get helpful links to resources mentioned

## Workflow

### Step 1 — Ensure yt-dlp is available

```bash
if [ ! -x /tmp/yt-dlp ]; then
  curl -sL https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp -o /tmp/yt-dlp && chmod +x /tmp/yt-dlp
fi
/tmp/yt-dlp --version
```

> yt-dlp is downloaded to `/tmp/yt-dlp` on first use. It is ~10 MB and takes about 5 seconds.

### Step 2 — Download the subtitle track

```bash
/tmp/yt-dlp \
  --write-auto-sub \
  --sub-lang en \
  --skip-download \
  --output /tmp/ytvideo \
  "<YOUTUBE_URL>"
```

This produces `/tmp/ytvideo.en.vtt`. If auto-subs are unavailable try `--write-sub` as fallback.

### Step 3 — Parse the VTT into clean transcript text

Run this Python snippet to strip timestamps, HTML tags, and duplicate lines:

```python
import re

with open('/tmp/ytvideo.en.vtt', 'r') as f:
    content = f.read()

lines = content.split('\n')
text_lines = []
seen = set()

for line in lines:
    line = line.strip()
    # Skip VTT metadata and timestamp lines
    if (re.match(r'^\d{2}:\d{2}', line)
            or line in ('WEBVTT', '')
            or line.startswith(('Kind:', 'Language:'))):
        continue
    # Strip inline HTML tags (e.g., <c>, <00:01:23.000>)
    clean = re.sub(r'<[^>]+>', '', line).strip()
    if clean and clean not in seen:
        seen.add(clean)
        text_lines.append(clean)

transcript = '\n'.join(text_lines)
print(transcript)
```

Save the output to `/tmp/transcript_clean.txt` for use in the next step.

### Step 4 — Extract video metadata (title + description)

```bash
curl -sL "https://www.youtube.com/watch?v=<VIDEO_ID>" | python3 -c "
import sys, re
html = sys.stdin.read()
t = re.search(r'\"title\":\{\"runs\":\[\{\"text\":\"(.*?)\"', html)
d = re.search(r'\"shortDescription\":\"(.*?)(?<!\\\\)\"', html, re.DOTALL)
if t: print('TITLE:', t.group(1))
if d: print('DESC:', d.group(1).replace('\\\\n','\n')[:1500])
"
```

This captures the title and description (which usually lists referenced links and channels).

### Step 5 — Produce the summary

With the full transcript text in context, write the summary using the **Output Format** below. Read the transcript carefully — do not make up content. Pull real section titles, timestamps, speaker names, and resources directly from the text.

---

## Output Format

Structure every summary exactly like this:

```markdown
## [Video Title]

**Video:** [Title](URL) by [Author/Channel]

---

### What It's About

1–3 sentence executive summary of the video's core subject and value.

---

### [Section Name] (~timestamp if available)

Concise description of what this section covers. Include any specific tools, techniques, 
commands, costs, or results mentioned by the speaker. Keep each section 2–5 sentences.

[Repeat for all major sections / use cases / topics]

---

### Key Takeaways

- Bullet 1
- Bullet 2
- Bullet 3
...

---

### Resources & Links

| Resource | Link |
|---|---|
| [Name] | [url] |
...
```

**Rules:**
- Use the real section/chapter names from the video, not generic headings
- Include approximate timestamps in parentheses when they appear in the transcript
- For each resource explicitly mentioned in the video (tools, repos, websites, courses), add it to the links table
- Keep the tone factual and informative — no hype
- If the video is technical, preserve specific commands, file names, config snippets
- If no transcript is available (private/geo-blocked video), say so clearly and explain the limitation

---

## Common Issues & Fixes

| Problem | Fix |
|---|---|
| `yt-dlp` download fails | The binary may be outdated. Delete `/tmp/yt-dlp` and re-download |
| No auto-subs available | Try `--write-sub` instead of `--write-auto-sub`; some channels only have manual subs |
| VTT file not found | Check yt-dlp output for the actual filename — language code may differ (e.g., `.en-orig.vtt`) |
| Transcript is garbled / no punctuation | Auto-subs from ASR are often unpunctuated; summarize meaning not exact words |
| Video is private or age-gated | yt-dlp cannot access private videos without cookies; inform the user |
| Non-English video | Try `--sub-lang` with the appropriate ISO 639-1 code (e.g., `es`, `fr`, `de`) |

---

## Example Invocations

```
Summarize this video: https://youtu.be/6KB73J01fGw
```

```
What is this YouTube video about? https://www.youtube.com/watch?v=dQw4w9WgXcQ
```

```
Give me the key takeaways from https://youtu.be/ABC123XYZ
```

---

## Getting Started

When the user provides a YouTube URL:

1. Run the yt-dlp install check (Step 1)
2. Download subtitles (Step 2)
3. Parse the VTT (Step 3)
4. Fetch title + description (Step 4)
5. Write the full summary using the Output Format (Step 5)

Do all heavy lifting (transcript fetch + parse) in Bash/Python tool calls before writing the summary. Never guess the video's content — always read the actual transcript.
