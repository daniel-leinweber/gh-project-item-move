# GitHub CLI Project Item Extension

<!--toc:start-->
- [GitHub CLI Project Item Extension](#github-cli-project-item-extension)
  - [Installation](#installation)
  - [Usage](#usage)
<!--toc:end-->

A [GitHub CLI](https://github.com/cli/cli) extension to move a project item to a specified Kanban column.

## Installation

**Prerequisite:**

- Install the [GitHub CLI](https://github.com/cli/cli).
- Make sure that you have read:project permissions

```bash
gh auth refresh -s project
```

**Install:**

```bash
gh extension install daniel-leinweber/gh-project-item-move
```

**Upgrade:**

```bash
gh extension upgrade project-item-move
```

**Uninstall:**

```bash
gh extension remove project-item-move
```

## Usage

The extension adds a `project-item-move` command to the GitHub CLI.

```bash
gh-project-item-move
Copyright (C) 2024 Daniel Leinweber
USAGE:
Move issue with ID '123' to the 'Done' column of the project 'MyProject':
  gh project-item-move --column Done --issue 123 --owner daniel-leinweber --project MyProject

  -o, --owner      Owner of the project (optional)

  -p, --project    Required. Name of the Project

  -i, --issue      Required. ID of the issue to move

  -c, --column     Required. Column Name to move the issue to

  --help           Display this help screen.

  --version        Display version information.
```
