# GitHub CLI Project Item Extension

A [GitHub CLI](https://github.com/cli/cli) extension to move a project item to a specified Kanban column.

<!--toc:start-->
- [Installation](#installation)
- [Usage](#usage)
<!--toc:end-->

## Installation

**Prerequisite:**

- Install the [GitHub CLI](https://github.com/cli/cli).
- Make sure that you have project permissions

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
Move issue interactively
  gh project-item-move
Move issue with ID '123' to the 'Done' column of the project 'MyProject':
  gh project-item-move --column Done --issue 123 --owner daniel-leinweber --project MyProject

  -o, --owner      Owner of the project (optional)

  -p, --project    Name of the Project (optional)

  -i, --issue      ID of the issue to move (optional)

  -c, --column     Column Name to move the issue to (optional)

  --help           Display this help screen.

  --version        Display version information.
```
