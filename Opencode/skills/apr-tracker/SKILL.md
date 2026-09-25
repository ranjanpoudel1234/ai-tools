---
name: apr-tracker
description: Aggregates monthly competency and business objective files into a polished Annual Performance Review (APR) document. Covers the September–August fiscal cycle. Creates two separate output files (competency + business objectives) inside competency-tracking/APR-{YEAR}/. AUTOMATICALLY INVOKE when user mentions "annual performance review", "APR", "create APR", "generate APR", "APR tracker", or "annual review".
---

# Annual Performance Review (APR) Aggregator

You are helping the user produce their Annual Performance Review by aggregating all monthly competency and business objective files for the fiscal year into two polished, deduplicated output documents.

## Your Role
1. Determine the APR fiscal year (September–August cycle)
2. Read all monthly competency and business objective markdown files for that period
3. Deduplicate entries within each framework
4. Show a pre-output coverage summary and flag any missing months
5. Generate two separate APR files inside `competency-tracking/APR-{YEAR}/`

---

## Key Constants

- **Root path**: `C:\Users\261906\.config\opencode\competency-tracking\`
- **Monthly folder format**: `YYYY-MM` (e.g., `2025-09`, `2026-08`)
- **Competency file pattern**: `competency-*.md`
- **Business objective file pattern**: `business-objectives-*.md`
- **APR output folder**: `C:\Users\261906\.config\opencode\competency-tracking\APR-{YEAR}\`
- **APR output files**:
  - `apr-competencies-FY{YEAR}.md`
  - `apr-business-objectives-FY{YEAR}.md`

---

## Fiscal Year Calendar

| APR Year | Covers |
|---|---|
| APR-2025 | Sep 2024 – Aug 2025 |
| APR-2026 | Sep 2025 – Aug 2026 |
| APR-2027 | Sep 2026 – Aug 2027 |

---

## Process Flow

### Step 1: Determine Fiscal Year
Ask the user: "Which APR year are you generating? (e.g., 2026 for the Sep 2025 – Aug 2026 cycle)"

Compute the 12 monthly folders to scan:
- For APR-YEAR: months are `{YEAR-1}-09` through `{YEAR}-08`
- Example for APR-2026: `2025-09`, `2025-10`, `2025-11`, `2025-12`, `2026-01`, `2026-02`, `2026-03`, `2026-04`, `2026-05`, `2026-06`, `2026-07`, `2026-08`

### Step 2: Scan and Read Monthly Files
For each of the 12 monthly folders:
1. Check if the folder exists at `C:\Users\261906\.config\opencode\competency-tracking\{YYYY-MM}\`
2. Look for competency files matching `competency-*.md`
3. Look for business objective files matching `business-objectives-*.md`
4. Read and parse the content of each file found
5. Track which months had files and which did not

**Note**: A missing monthly folder does NOT mean the data is missing — the user may have combined multiple months into a single file (e.g., a file covering October–November). Flag missing months as warnings only and continue.

### Step 3: Show Pre-Output Coverage Summary
Before generating any output, display a summary table:

```
## APR-{YEAR} Coverage Summary (Sep {YEAR-1} – Aug {YEAR})

| Month | Competency File | Business Objective File |
|-------|-----------------|------------------------|
| Sep {YEAR-1} | ✅ found | ✅ found |
| Oct {YEAR-1} | ⚠️ not found | ⚠️ not found |
| Nov {YEAR-1} | ✅ found | ✅ found |
| ... | ... | ... |
| Aug {YEAR} | ✅ found | ✅ found |

**Competency entries found**: [X] raw bullets across [N] files
**Business objective entries found**: [X] raw bullets across [N] files
**Missing months**: [list] — these may be combined into other files

Do you want to add any notes for missing months before I generate the APR files, or shall I proceed?
```

Wait for user confirmation or additional notes before proceeding.

### Step 4: Aggregate and Deduplicate Competency Entries

For the competency output, collect all bullet points grouped by competency category (CL, TW, A&D, COMM, CS, PE, M, TS).

**Deduplication rules (within competency output only):**
- Consider two bullets duplicates if they describe the same specific activity/accomplishment
- Keep the most detailed/professional version when duplicates are found
- Slightly different framings of the same work are acceptable — keep both only if they add genuinely different insight
- If in doubt, keep the entry rather than silently dropping it

**Enhancement:**
- After aggregating, use strong action verbs and professional language
- Where multiple months show progression on the same initiative, combine into one impactful statement that shows the arc (e.g., "Designed and delivered X over Q1-Q3, resulting in Y")
- Quantify impact wherever metrics appear in source files

### Step 5: Aggregate and Deduplicate Business Objective Entries

For the business objective output, collect all bullet points grouped by sub-criteria (1A, 1B, 1C, 2A, 2B, 3A, 3B, 3C).

**Same deduplication rules as Step 4, applied independently to this output.**

**Note**: An activity that appears in both a competency file AND a business objective file should appear in BOTH output files — same work, different lenses. Deduplication is only within each output file.

### Step 6: Generate APR Competency File

Create `C:\Users\261906\.config\opencode\competency-tracking\APR-{YEAR}\apr-competencies-FY{YEAR}.md`:

```markdown
# Annual Performance Review — Competencies FY{YEAR}
**Employee ID**: 261906
**Period**: September {YEAR-1} – August {YEAR}

---

## Courageous Leadership (CL)
- [Professional statement 1]
- [Professional statement 2]

## Teamwork (TW)
- [Professional statement 1]
- [Professional statement 2]

## Analysis & Decision Making (A&D)
- [Professional statement 1]
- [Professional statement 2]

## Communication (COMM)
- [Professional statement 1]
- [Professional statement 2]

## Customer Service (CS)
- [Professional statement 1]
- [Professional statement 2]

## Planning & Execution (PE)
- [Professional statement 1]
- [Professional statement 2]

## Mentoring (M)
- [Professional statement 1]
- [Professional statement 2]

## Technical Skills / Subject Matter Expert (TS)
- [Professional statement 1]
- [Professional statement 2]

---

## Summary
[2-3 sentence paragraph highlighting key themes across all competencies, overall impact, and demonstration of professional growth over the year]

---

## Source Coverage
- **Months with competency files**: [list]
- **Months flagged as missing**: [list]
- **Total source bullets processed**: [X]
- **Total bullets in this APR**: [Y]
- **Duplicates removed**: [Z]

---
*Generated: {Current Date}*
*Ready for Annual Performance Review submission*
```

### Step 7: Generate APR Business Objectives File

Create `C:\Users\261906\.config\opencode\competency-tracking\APR-{YEAR}\apr-business-objectives-FY{YEAR}.md`:

```markdown
# Annual Performance Review — Business Objectives FY{YEAR}
**Employee ID**: 261906
**Period**: September {YEAR-1} – August {YEAR}
**Theme**: Year of "Excellence" through E3 (Experience, Efficiency, Execution)

---

## #1 - Deliver on Business Priorities while Strengthening Technology Foundation

### 1A. Deliver Key Initiatives and Support Larger Initiatives
- [Professional statement 1]
- [Professional statement 2]

### 1B. Improve Delivery Capabilities
- [Professional statement 1]
- [Professional statement 2]

### 1C. Lead Innovation Research, Development and Best Practices
- [Professional statement 1]
- [Professional statement 2]

---

## #2 - Provide Predictable, Reliable and Secure Operation of Technology while Improving Efficiencies and Costs

### 2A. Improve Total Cost of Ownership and Create SG&A Leverage
- [Professional statement 1]
- [Professional statement 2]

### 2B. Improve Overall Quality, Security and Reliability
- [Professional statement 1]
- [Professional statement 2]

---

## #3 - Strengthen the Technology Organization through Developing and Attracting Talent

### 3A. Actively Participate in and Lead Associate Engagement
- [Professional statement 1]
- [Professional statement 2]

### 3B. Enable Talent Management Culture of Development, Growth and Learning
- [Professional statement 1]
- [Professional statement 2]

### 3C. Increased Focus on Performance and Development of Existing Talent, While Attracting Top Talent
- [Professional statement 1]
- [Professional statement 2]

---

## E3 Excellence Highlights
**Experience**: [1-2 sentences on how work delivered iconic experiences]
**Efficiency**: [1-2 sentences on how work achieved more with less]
**Execution**: [1-2 sentences on relentless execution through innovative technology]

---

## Summary
[2-3 sentence paragraph highlighting key themes across all 3 Business Objectives, alignment with mission "Together we deliver iconic experiences through innovative technology and relentless execution", and overall annual impact]

---

## Progress Tracker
- **Objective #1 (Deliver Business Priorities)**: [X] examples across 1A, 1B, 1C
- **Objective #2 (Reliable Operations)**: [X] examples across 2A, 2B
- **Objective #3 (Strengthen Organization)**: [X] examples across 3A, 3B, 3C

## Source Coverage
- **Months with business objective files**: [list]
- **Months flagged as missing**: [list]
- **Total source bullets processed**: [X]
- **Total bullets in this APR**: [Y]
- **Duplicates removed**: [Z]

---
*Generated: {Current Date}*
*Ready for Annual Performance Review submission*
```

### Step 8: Verify and Report
After saving both files, confirm:
1. Both files saved successfully — show full file paths
2. Final verification checklist:
   - Total source entries read: [X competency] + [Y business objective]
   - Total APR entries written: [A competency] + [B business objective]
   - Duplicates removed: [C competency] + [D business objective]
   - Any entries that could not be mapped to a category (flag these for user review)

### Step 9: Iterate if Needed
Ask: "Would you like to add any additional accomplishments, adjust categorizations, or refine any statements before finalizing?"

---

## Competency Model Reference

### CL - Courageous Leadership
Expresses pride and enthusiasm. Influences others to do the right thing. Leads by example. Gets buy-in from others. Motivates and leads peers, initiates new projects or methods, embraces change, connects team goals with larger department goals.

### TW - Teamwork
Models company values: integrity, respect, fun, inclusion, fairness. Builds strong partnerships, resolves conflict with win-win outcomes, shares information, contributes to everyone's success, capitalizes on team strengths.

### A&D - Analysis & Decision Making
Goes beyond the obvious. Root-cause analysis, organizational thinking, facilitates consensus, applies business knowledge to solve problems, evaluates diverse ideas and solutions.

### COMM - Communication
Communicates clearly through verbal, written, and non-verbal methods. Speaks effectively in groups, adapts style to audience, active listening, engaging presentations, facilitates group discussions.

### CS - Customer Service
Strong internal and external customer service. Prioritizes customer commitments, seeks win-win solutions, delivers on commitments.

### PE - Planning & Execution
Strong planning and execution. Hits deadlines, manages multiple projects, sets high quality standards, owns work, adjusts plans, identifies resources, fiscal responsibility.

### M - Mentoring
Empowers team members to reach full potential. Offers developmental feedback, teachable moments, encourages others to exceed expectations, models personal growth, guides using experience and knowledge.

### TS - Technical Skills / Subject Matter Expert
Knows the business and is a go-to SME. Uses appropriate tools, identifies process and system improvements, applies technical knowledge to produce results.

---

## Business Objectives Framework Reference

### Objective #1: Deliver Business Priorities while Strengthening Technology Foundation
- **1A**: Key initiatives (SO Mod, Sublet Management, LUPE, Driver App, Inventory Hub, Data Center Migration, APIM, etc.)
- **1B**: Delivery capabilities (Developer Excellence, automation, CI/CD, automated testing, code quality, DevOps)
- **1C**: Innovation R&D (AI/GenAI, Innovation Garage, DevOps Days, D2U, technical spikes, prototyping)

### Objective #2: Predictable, Reliable and Secure Operation
- **2A**: Cost/efficiency (cloud optimization, storage, productivity, cost reduction, achieve more with less)
- **2B**: Quality/security/reliability (monitoring, Application Insights, SLAs/SLOs, security, error handling, resilience)

### Objective #3: Strengthen the Technology Organization through Talent
- **3A**: Associate engagement (AVS action plans, team building, retrospectives, culture, hybrid work)
- **3B**: Development/growth/learning (learning agenda, IDP, certifications, mentoring, knowledge sharing)
- **3C**: Performance/talent (interviews, referrals, onboarding, performance feedback, succession planning)

---

## Language Transformation Guidelines

Use strong action verbs: Led, Drove, Delivered, Implemented, Designed, Optimized, Resolved, Mentored, Facilitated, Established, Improved, Pioneered, Championed, Accelerated, Streamlined.

Quantify impact where possible. Use STAR format for complex accomplishments. Emphasize outcomes, not just activities. Use past tense consistently.

**Input**: "Worked on Project Alpha modernization"
**Output**: "Led architectural research and boundary system analysis for Project Alpha Modernization initiative, defining integration contracts and accelerating team alignment on target state design"

**Input**: "Helped team with deployments"
**Output**: "Provided cross-team deployment guidance and mentorship, reducing deployment friction and building team capability for independent delivery"

## Ready to Begin
When invoked, start with Step 1: ask the user which APR year they are generating.
