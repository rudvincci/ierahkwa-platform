# Mamey GitHub Repository Management Script

This script automates various GitHub repository management tasks, including cloning repositories, creating feature branches, creating new repositories, pushing existing repositories, committing features, and creating pull requests across multiple repositories. It supports both long and short command options for ease of use.

## Prerequisites

- GitHub CLI (`gh`) installed and authenticated with your GitHub account.
- Bash environment for running the script.

## Configuration

Before using the script, set your GitHub organization/user name by modifying the `GITHUB_ORG` variable at the top of the script:

```bash
GITHUB_ORG="YourGitHubOrgName"
```

Also, ensure the default repository list file `mamey-code-all.txt` is present in the same directory as the script or specify a custom list file using the `--repo-list-file` option.

## Running the script

To run the script, save it to a file, give it execute permissions with `chmod +x mamey-git.sh`, and then navigate to the root level of the folder to clone all repositories and run it with `./Mamey.Info/scripts/mamey-git.sh --command <commandValue> [options]`.

## Commands and Options

The script supports several commands, each designed for specific repository management tasks. Below are the commands, their options, and usage examples.

### clone_repositories

Clones all repositories listed in the specified repository list file.

**Usage:**

```bash
./mamey-git.sh --command clone_repositories
```

**Short Option:**

```bash
./mamey-git.sh -c clone_repositories
```

### create_feature_branch

Creates a new feature branch in all listed repositories.

**Options:**

- `--branch-name <branch>`: Name of the feature branch to create.
- `-b <branch>`: Short option for `--branch-name`.

**Usage:**

```bash
./mamey-git.sh --command create_feature_branch --branch-name feature-branch
```

**Short Option:**

```bash
./mamey-git.sh -c create_feature_branch -b feature-branch
```

### create_new_repository

Creates a new repository on GitHub and initializes it with a .NET microservice template.

**Options:**

- `--repo-name <name>`: Name of the new repository.
- `--visibility <public|private>`: Repository visibility setting.
- `-n <name>`: Short option for `--repo-name`.
- `-v <public|private>`: Short option for `--visibility`.

**Usage:**

```bash
./mamey-git.sh --command create_new_repository --repo-name MyNewRepo --visibility private
```

**Short Option:**

```bash
./mamey-git.sh -c create_new_repository -n MyNewRepo -v private
```

### push_existing_repositories

Pushes all existing repositories to GitHub, creating the repository if it doesn't exist.

**Usage:**

```bash
./mamey-git.sh --command push_existing_repositories
```

**Short Option:**

```bash
./mamey-git.sh -c push_existing_repositories
```

### commit_feature

Commits changes in a feature branch for all repositories with an option to push.

**Options:**

- `--branch-name <branch>`: Name of the feature branch.
- `--commit-message <message>`: Commit message.
- `--push-repo <yes|no>`: Whether to push the commit to the remote repository.
- `-b <branch>`: Short option for `--branch-name`.
- `-m <message>`: Short option for `--commit-message`.
- `-p <yes|no>`: Short option for `--push-repo`.

**Usage:**

```bash
./mamey-git.sh --command commit_feature --branch-name feature-branch --commit-message "Add new feature" --push-repo yes
```

**Short Option:**

```bash
./mamey-git.sh -c commit_feature -b feature-branch -m "Add new feature" -p yes
```

### create_pull_requests

Creates pull requests from a feature branch to a base branch for all repositories.

**Options:**

- `--head-branch <branch>`: The feature branch from which to create pull requests.
- `--base-branch <branch>`: The base branch into which the pull requests will be merged.
- `--pr-title <title>`: Title for the pull requests.
- `-h <branch>`: Short option for `--head-branch`.
- `-b <branch>`: Short option for `--base-branch`.
- `-t <title>`: Short option for `--pr-title`.

**Usage:**

```bash
./mamey-git.sh --command create_pull_requests --head-branch feature-branch --base-branch main --pr-title "Feature implementation"
```

**Short Option:**

```bash
./mamey-git.sh -c create_pull_requests -h feature-branch -b main -t "Feature implementation"
```

### commit_and_push_repositories

Commits and optionally pushes changes for all repositories listed in the repository list file.

**Options:**

- `--commit-message <message>`: Commit message.
- `--push-repo <yes|no>`: Whether to push the commit to the remote repository.
- `-m <message>`: Short option for `--commit-message`.
- `-p <yes|no>`: Short option for `--push-repo`.

**Usage:**

```bash
./mamey-git.sh --command commit_and_push_repositories --commit-message "General updates" --push-repo yes
```

**Short Option:**

```bash
./mamey-git.sh -c commit_and_push_repositories -m "General updates" -p yes
```

### Help

To display help information for a specific command, use the `--help` or `-h` option followed by the command.

**Usage:**

```bash
./mamey-git.sh --command <command> --help
```

or

```bash
./mamey-git.sh -c <command> -h
```

**Example:**

```bash
./mamey-git.sh --command clone_repositories --help
```

## Repository List File

The script operates on repositories listed in a file specified by the `--repo-list-file` option or the default `mamey-code-all.txt`. Each repository name should be on a separate line.

```
// mamey-code-all.txt

Repository1
Repository2
...
EOF
```