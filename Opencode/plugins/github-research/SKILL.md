---
name: github-research
description: Search and analyze GitHub repositories, code, pull requests, issues, and commits using GitHub CLI. AUTOMATICALLY INVOKE when user mentions 'search github', 'find repo', 'github search', 'lookup github', 'search code in github', or asks about your-org repositories.
license: MIT
compatibility: opencode
metadata:
  version: "1.0.0"
  author: "261906"
  category: "development"
  tags: "github,research,search,your-org"
  last_updated: "2024-02-13"
---

# GitHub Research Assistant

**Purpose**: Expert assistant for researching GitHub repositories, code, and activity using GitHub CLI (`gh`). Specializes in your-org organization but works with any GitHub repository.

---

## 🎯 What This Skill Does

This skill helps you research and explore GitHub efficiently by:

1. **Searching Repositories** - Find repos by name, description, topics, or keywords
2. **Searching Code** - Locate code patterns, classes, functions, or configurations
3. **Viewing Repository Details** - Get metadata, structure, README, and languages
4. **Listing Pull Requests** - Find and filter PRs by state, author, or labels
5. **Listing Issues** - Search and filter issues across repositories
6. **Viewing Commits** - Check commit history and recent activity
7. **Analyzing Repository Activity** - Recent updates, contributors, stats

---

## 🚀 When to Use This Skill (Automatic Triggers)

**Automatically invokes when user mentions:**
- "search github" / "github search"
- "find repo" / "find repository"
- "lookup github"
- "search code in github"
- "github code search"
- "find PRs" / "list pull requests"
- "github issues"
- "your-org repositories"
- "search your-org"
- "which repos use [X]"

**Also invoke when:**
- User needs to explore your-org organization
- User wants to find repositories using specific technologies
- User needs to search for code patterns across repos
- User wants to check PR or issue status
- User needs repository activity information

---

## 📚 Core Capabilities

You can help users with:

### Repository Research
- **Search repos** by name, description, topics, or keywords
- **View repo details** including README, languages, stats
- **List repos** in your-org organization
- **Filter repos** by language, topic, or visibility
- **Get repo metadata** (stars, forks, last update, creation date)

### Code Search
- **Search code** across repositories
- **Find specific patterns** (classes, functions, configs)
- **Locate implementations** of particular features
- **Search by language** or file type
- **Find usage examples** of APIs or libraries

### Pull Request Research
- **List PRs** by state (open, closed, merged)
- **Filter by author** or reviewer
- **Search by labels** or milestone
- **Get PR details** including changes and comments
- **Check PR status** and CI results

### Issue Tracking
- **Search issues** across repos
- **Filter by state, labels, assignee**
- **Get issue details** and comments
- **Track issue activity**

### Commit History
- **View commit history** for repos
- **Search commits** by author or message
- **Get commit details** including changes
- **Check recent activity**

### Repository Activity
- **Recent updates** across organization
- **Contributor activity**
- **Repository statistics**
- **Language distribution**

---

## 🔧 GitHub CLI Configuration

**Prerequisites**: GitHub CLI (`gh`) must be installed and authenticated

**Authentication Status**: You are currently authenticated as `your-github-user`
- Token scopes: `read:org`, `read:user`, `repo`
- Connected to: github.com

**Path**: `~/bin/gh` (ensure this is in your PATH)

**Test Connection**:
```bash
export PATH="$HOME/bin:$PATH" && gh auth status
```

---

## 📖 GitHub CLI Command Reference

### Repository Search Commands

#### Search repositories by keyword
```bash
gh search repos [keyword] --owner your-org --limit 50
```

**Examples**:
```bash
# Search for related repos
gh search repos "ero" --owner your-org --limit 50

# Search for specific service
gh search repos "vehicle-maintenance" --owner your-org --limit 20

# Search with language filter
gh search repos "api" --owner your-org --language csharp --limit 30
```

#### View repository details
```bash
gh repo view [owner/repo]
```

**Examples**:
```bash
# View repo details
gh repo view your-org/logistics-shipment-service

# Get JSON output
gh repo view your-org/ero-modernization-research --json name,description,url,languages,createdAt,updatedAt
```

#### List organization repositories
```bash
gh repo list your-org --limit 100
```

**With filters**:
```bash
# Filter by language
gh repo list your-org --language csharp --limit 50

# Filter by topic
gh repo list your-org --topic ero --limit 30

# Get JSON output with specific fields
gh repo list your-org --json name,description,languages,updatedAt --limit 100
```

---

### Code Search Commands

#### Search code across repositories
```bash
gh search code [query] --owner your-org --limit 50
```

**Examples**:
```bash
# Search for specific class or interface
gh search code "EroHeader" --owner your-org --limit 30

# Search for code pattern
gh search code "ICommandHandler" --owner your-org --language csharp --limit 20

# Search in specific file types
gh search code "mitchell" --owner your-org --language csharp --limit 50

# Search with path filter
gh search code "appsettings" --owner your-org --extension json --limit 30
```

**Query Syntax**:
- `"exact match"` - Exact phrase
- `language:csharp` - Filter by language
- `path:/src/` - Search in specific paths
- `filename:Program.cs` - Search in specific files
- `extension:md` - Search by file extension
- `org:your-org` - Organization scope

---

### Pull Request Commands

#### Search pull requests
```bash
gh search prs [query] --owner your-org --limit 30
```

**Examples**:
```bash
# Find open PRs
gh search prs "is:open" --owner your-org --limit 20

# Find PRs by author
gh search prs "author:your-github-user" --owner your-org --limit 30

# Find merged PRs
gh search prs "is:merged" --owner your-org --limit 20

# Search PR titles
gh search prs "fix authentication" --owner your-org --state all --limit 20
```

#### List PRs for specific repo
```bash
gh pr list --repo your-org/[repo-name]
```

**Examples**:
```bash
# List all open PRs
gh pr list --repo your-org/logistics-shipment-service --state open

# List all PRs (including closed)
gh pr list --repo your-org/ero-modernization-research --state all --limit 50
```

#### View PR details
```bash
gh pr view [number] --repo your-org/[repo-name]
```

---

### Issue Commands

#### Search issues
```bash
gh search issues [query] --owner your-org --limit 30
```

**Examples**:
```bash
# Find open issues
gh search issues "is:open" --owner your-org --limit 30

# Find issues by label
gh search issues "label:bug" --owner your-org --limit 20

# Find issues assigned to someone
gh search issues "assignee:your-github-user" --owner your-org --limit 20
```

#### List issues for specific repo
```bash
gh issue list --repo your-org/[repo-name]
```

---

### Commit & Activity Commands

#### View recent commits
```bash
gh api repos/your-org/[repo-name]/commits --paginate --per-page 20
```

**Example with jq parsing**:
```bash
gh api repos/your-org/logistics-shipment-service/commits \
  --jq '.[] | {message: .commit.message, author: .commit.author.name, date: .commit.author.date}'
```

#### Get repository activity
```bash
gh api repos/your-org/[repo-name]
```

---

## 🎨 Workflow Patterns

### Pattern 1: User asks "Find repos that use [technology/entity]"

**Steps**:
1. Determine the search term (e.g., "EroHeader", "MitchellData")
2. Use code search to find repos: `gh search code "[term]" --owner your-org`
3. Extract unique repository names from results
4. Present list of repositories with brief context
5. Offer to get more details on specific repos

**Example Flow**:
```
User: "Which repositories use EroHeader entity?"