---
name: summarize-transcript-skill
description: Extracts and summarizes content from meeting transcripts, Word documents, or text files. AUTOMATICALLY INVOKE when user mentions "summarize transcript", "extract from transcript", "process meeting notes", "summarize meeting", or "analyze transcript". Asks the user what specifically they want extracted before processing, accepting a custom user prompt to guide extraction.
license: MIT
compatibility: opencode
metadata:
  version: "1.1.0"
  author: "ranjan"
  category: "productivity"
  tags: "transcript,summarize,meetings,extract,notes"
---

# Summarize Transcript Skill

## Purpose

Extracts targeted information from meeting transcripts, Word documents (.docx), or plain text files. Before processing, this skill **always asks the user what they want extracted**, accepting a custom prompt that drives the extraction — so every run is tailored to the user's exact need.

---

## When to Use

- You have a transcript or meeting notes file (`.docx`, `.txt`, `.md`)
- You want to extract specific things: action items, decisions, technical details, risks, key quotes, etc.
- You want a structured summary shaped by your own question or prompt

**Auto-invoke keywords**: "summarize transcript", "extract from transcript", "process meeting notes", "analyze transcript", "summarize meeting", "extract from meeting"

---

## Getting Started (Initial Flow)

When this skill is invoked:

1. **Greet the user and ask for the file path** (if not already provided):
   > "What is the path to the transcript or document you'd like me to process?"

2. **Ask the user what they want extracted** — this is the custom user prompt:
   > "What specifically would you like me to extract or focus on? For example:
   > - 'Give me all action items with owners'
   > - 'Summarize the key decisions made'
   > - 'Extract all technical risks and blockers'
   > - 'List open questions and who raised them'
   > - 'Give me a full structured summary'
   > - Or type your own custom prompt."

3. **Confirm before processing**:
   > "Got it. I'll extract: [repeat user's prompt back]. Processing `[filename]` now..."

4. **Convert and read the document**:
   - For `.docx` files: use the Python `zipfile` + `xml.etree` method (see "Converting Word Documents to Markdown") to convert to `.md` first, then use the Read tool on the output file
   - For `.txt` / `.md` files: use the Read tool directly

5. **Apply the user's prompt** to the content — extract exactly what was asked for, structured clearly in Markdown.

6. **Present results** in clean Markdown with labeled sections.

7. **Ask if they want anything else** from the same document:
   > "Would you like me to extract anything else from this transcript?"

---

## Converting Word Documents to Markdown (PREFERRED METHOD)

**Always convert `.docx` files to Markdown first before extracting content.** This ensures clean, readable text that can be processed with the Read tool.

### Step 1: Convert .docx to .md using Python stdlib (no third-party libraries needed)

This method uses only Python's built-in `zipfile` and `xml.etree.ElementTree` — no `pip install` required. A `.docx` file is a ZIP archive containing `word/document.xml`.

```python
import zipfile
from xml.etree import ElementTree as ET

docx_path = '/path/to/your/file.docx'
output_path = '/path/to/output.md'

try:
    with zipfile.ZipFile(docx_path, 'r') as zip_file:
        xml_content = zip_file.read('word/document.xml')

    tree = ET.fromstring(xml_content)
    namespace = {'w': 'http://schemas.openxmlformats.org/wordprocessingml/2006/main'}

    paragraphs = []
    for paragraph in tree.findall('.//w:p', namespace):
        texts = []
        for text in paragraph.findall('.//w:t', namespace):
            if text.text:
                texts.append(text.text)
        if texts:
            paragraphs.append(''.join(texts))

    with open(output_path, 'w', encoding='utf-8') as f:
        f.write('\n\n'.join(paragraphs))

    print(f"Converted successfully: {output_path}")
    print(f"Total paragraphs: {len(paragraphs)}")
except Exception as e:
    print(f"Error: {e}")
```

**Run it via Bash:**
```bash
python3 /tmp/convert_docx.py
```

Or inline as a one-shot Bash command (write the script to a temp file first, then run):
```bash
python3 - <<'EOF'
import zipfile
from xml.etree import ElementTree as ET

docx_path = '/path/to/your/file.docx'
output_path = '/tmp/converted-output.md'

with zipfile.ZipFile(docx_path, 'r') as zip_file:
    xml_content = zip_file.read('word/document.xml')

tree = ET.fromstring(xml_content)
namespace = {'w': 'http://schemas.openxmlformats.org/wordprocessingml/2006/main'}

paragraphs = []
for paragraph in tree.findall('.//w:p', namespace):
    texts = [t.text for t in paragraph.findall('.//w:t', namespace) if t.text]
    if texts:
        paragraphs.append(''.join(texts))

with open(output_path, 'w', encoding='utf-8') as f:
    f.write('\n\n'.join(paragraphs))

print(f"Done: {output_path} ({len(paragraphs)} paragraphs)")
EOF
```

### Step 2: Read the converted .md file

After conversion, use the Read tool to read the `.md` file and apply the user's extraction prompt to it.

### Naming the output file

Derive the output filename from the source `.docx` name:
- Strip the `.docx` extension
- Replace spaces with hyphens
- Lowercase all characters
- Append `.md`

Example: `SO Mod - AI Session 2.docx` → `so-mod-ai-session-2.md`

---

## Reading Other File Types

### Plain Text / Markdown (.txt, .md)
Use the Read tool directly — no conversion needed.

### PDF Files (.pdf)
```bash
pip install pdfminer.six
python3 -c "
from pdfminer.high_level import extract_text
print(extract_text('/path/to/file.pdf'))
"
```

---

## Applying the User's Custom Prompt

Once the document content is extracted, apply the user's prompt as the lens for analysis. Examples of how to interpret common prompts:

| User Prompt | What to Extract |
|---|---|
| "Action items with owners" | Bullet list of tasks + who is responsible + due dates if mentioned |
| "Key decisions" | Numbered list of decisions made, with context |
| "Technical risks and blockers" | Risks, dependencies, blockers, unresolved issues |
| "Open questions" | Unanswered questions, raised by whom if stated |
| "Full structured summary" | Date, attendees, agenda, decisions, action items, next steps |
| "Key quotes" | Verbatim notable quotes with speaker attribution |
| "Executive summary" | 3–5 bullet point high-level overview |
| Any custom prompt | Extract as specifically as the prompt instructs |

---

## Output Format

Structure results clearly in Markdown. Example for a "full structured summary" prompt:

```markdown
## Meeting Summary — [Date or Document Name]

### Attendees
- Name 1
- Name 2

### Key Decisions
1. Decision A
2. Decision B

### Action Items
| # | Task | Owner | Due |
|---|------|-------|-----|
| 1 | Do X | Alice | Apr 15 |
| 2 | Fix Y | Bob | TBD |

### Open Questions
- Question 1 (raised by: ?)
- Question 2

### Key Discussion Topics
- Topic A
- Topic B

### Next Steps
- Step 1
- Step 2
```

For targeted prompts (e.g., "just give me action items"), only output the relevant section — do not add unnecessary structure.

---

## Handling Multiple Files

If the user provides a folder path, process each file in the folder and either:
- Apply the same prompt to all files and produce individual summaries
- Produce a **consolidated cross-document summary** if the user requests it

Ask first:
> "I found [N] files. Should I summarize each one separately, or produce a single consolidated summary across all of them?"

---

## Error Handling

| Issue | Response |
|---|---|
| File not found | "I couldn't find the file at `[path]`. Please double-check the path." |
| Unsupported file type | "This file type isn't supported directly. Supported formats: .docx, .txt, .md, .pdf" |
| Empty document | "The document appears to be empty or couldn't be read. Try converting it to `.txt` first." |
| `.docx` conversion fails | Check the file path is correct and the file is not password-protected; the stdlib method requires no pip installs |
| Ambiguous user prompt | Ask a clarifying follow-up question before extracting |

---

## Examples

### Example 1: Single Action Items Extraction
```
User: summarize transcript — file: C:\Users\me\Downloads\meeting.docx
User prompt: Give me all action items with owners and due dates
```
Output: Clean Markdown table of action items only.

### Example 2: Full Summary
```
User: extract from transcript C:\meetings\planning-session.docx
User prompt: Full structured summary
```
Output: Full structured Markdown summary with all sections.

### Example 3: Custom Technical Prompt
```
User: analyze transcript — data-architecture-review.docx
User prompt: Extract all mentions of data pipeline decisions, tools chosen, and integration concerns
```
Output: Targeted extraction of pipeline decisions, tools, and integration issues only.

### Example 4: Folder of Documents
```
User: summarize all transcripts in C:\Users\me\Downloads\AI Meetings
User prompt: Key decisions and action items from each meeting
```
Output: Per-meeting summaries with decisions and action items, then a cross-meeting rollup.

---

## Best Practices

- Always confirm the user's prompt before processing — never guess what they want
- Keep output lean and targeted; do not pad with unnecessary sections
- If the transcript is long, scan for structural cues (headings, speaker labels, timestamps) to improve extraction quality
- If speaker attribution is present in the transcript, preserve it in the output
- Always ask at the end if the user wants anything else extracted

---

## Quick Reference

```
1. Get file path from user
2. Ask: "What specifically do you want extracted?"
3. Confirm prompt with user
4. If .docx: convert to .md using Python zipfile + xml.etree (no pip needed)
   If .txt/.md: read directly with Read tool
5. Apply user's prompt to the converted/read content
6. Output structured Markdown results
7. Ask if anything else is needed
```
