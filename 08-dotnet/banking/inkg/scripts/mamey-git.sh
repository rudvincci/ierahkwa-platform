#!/bin/bash

# Default repository list file
repo_list_file="Mamey.Info/scripts/mamey-code-all.txt"

# Global settings or actions
while [[ $# -gt 0 ]]; do
    case "$1" in
        --repo-list-file)
            repo_list_file=$2
            shift 2
            ;;
        *)
            # If not a global setting, break to preserve remaining arguments for commands
            break
            ;;
    esac
done

# Ensure the repository list file exists
if [[ ! -f "$repo_list_file" ]]; then
    echo "Repository list file not found: $repo_list_file"
    echo ${PWD}
    exit 1
fi

# Read repository names into an array from the specified repo list file
REPOSITORIES=() # Initialize empty array
while IFS= read -r line; do
    REPOSITORIES+=("$line")
done < "$repo_list_file"

# GitHub organization/user name
GITHUB_ORG="Mamey-io"

# Function to clone repositories
clone_repositories() {
    for repo in "${REPOSITORIES[@]}"; do
        echo "Cloning repository: $repo"
        gh repo clone "$GITHUB_ORG/$repo"
    done
}

# Function to create a new feature branch in all repositories
create_feature_branch() {
    local branch_name=$1
    for repo in "${REPOSITORIES[@]}"; do
        echo "Processing repository: $repo"
        
        # Ensure repository is cloned
        local repo_path="./$repo"
        if [ ! -d "$repo_path" ]; then
            echo "Repository $repo does not exist locally. Cloning..."
            gh repo clone "$GITHUB_ORG/$repo"
        fi
        
        cd "$repo" || exit
        
        # Create the feature branch and push it to the remote repository
        git checkout -b "$branch_name"
        git push -u origin "$branch_name"
        
        cd ..
    done
}


# Function to create a new repository and initialize with .NET microservice template
create_new_repository() {
    local repo_name=$1
    local visibility=$2
    gh repo create "$GITHUB_ORG/$repo_name" --${visibility} --confirm
    mkdir "$repo_name" && cd "$repo_name"
    dotnet new mamey-micro --name "$repo_name"
    git init
    git branch -M master
    git add .
    git commit -m "Initial .NET microservice setup"
    git remote add origin "https://github.com/$GITHUB_ORG/$repo_name.git"
    git push -u origin master
    cd ..
}

# Function to push all existing repositories to GitHub, creating the repo if it doesn't exist
push_existing_repositories() {
    for repo_name in "${REPOSITORIES[@]}"; do
        echo "Processing repository: $repo_name"
        
        local repo_path="./${repo_name}" # Assuming each repo is in a directory named after the repo within the current directory
        
        if [ -d "$repo_path" ] && [ "$(ls -A "$repo_path")" ]; then
            cd "$repo_path"
            
            if [ ! -d ".git" ]; then
                echo "Initializing new Git repository for $repo_name..."
                git init
            fi
            
            if ! gh repo view "$GITHUB_ORG/$repo_name" >/dev/null 2>&1; then
                echo "Repository $repo_name does not exist. Creating it..."
                gh repo create "$GITHUB_ORG/$repo_name" --private --confirm
            fi
            
            git remote add origin "https://github.com/$GITHUB_ORG/$repo_name.git" || git remote set-url origin "https://github.com/$GITHUB_ORG/$repo_name.git"
            git branch -M master
            git add .
            git commit -m "Initial commit" --allow-empty
            git push -u origin master
            
            cd ..
        else
            echo "Directory for $repo_name does not exist or is empty. Skipping..."
        fi
    done
}

# Function to commit and push changes for all repositories
commit_and_push_repositories() {
    local commit_message=$1
    for repo_dir in "${REPOSITORIES[@]}"; do
        echo "Processing $repo_dir..."
        local repo_path="./$repo_dir"
        if [ -d "$repo_path" ]; then
            cd "$repo_path" || continue
            
            # Check if there are any changes to commit
            if git status --porcelain | grep -q "^"; then
                git add .
                git commit -m "$commit_message"
                
                if [[ $2 == "yes" ]]; then
                    git push
                    echo "Changes pushed for $repo_dir."
                else
                    echo "Changes committed for $repo_dir. Not pushed."
                fi
            else
                echo "No changes to commit for $repo_dir."
            fi
            
            cd ..
        else
            echo "Directory $repo_dir does not exist. Skipping..."
        fi
    done
}
# Function to stage all changes in all repositories
stage_all_changes() {
    for repo_dir in "${REPOSITORIES[@]}"; do
        echo "Processing $repo_dir for staging changes..."
        local repo_path="./$repo_dir"
        
        if [ -d "$repo_path" ]; then
            cd "$repo_path" || { echo "Failed to enter $repo_dir, skipping..."; continue; }

            # Check if it's a git repository
            if git rev-parse --git-dir > /dev/null 2>&1; then
                # Stage all changes
                git add .
                echo "All changes staged in $repo_dir."
            else
                echo "$repo_dir is not a git repository. Skipping..."
            fi
            
            cd ..
        else
            echo "Directory $repo_dir does not exist. Skipping..."
        fi
    done
}

# Function to handle git operations for a commit feature with option to push
commit_feature() {
    local branch_name=$1
    local commit_message=$2
    local push_repo=$3
    for repo_dir in "${REPOSITORIES[@]}"; do
        echo "Processing $repo_dir..."
        local repo_path="./$repo_dir"
        
        cd "$repo_path" || { echo "$repo_dir not found, skipping."; continue; }
        
        # Check if the branch exists and switch to it
        if git rev-parse --verify "$branch_name" >/dev/null 2>&1; then
            git checkout "$branch_name"
        else
            echo "Branch $branch_name does not exist in $repo_dir, skipping."
            cd ..
            continue
        fi
        
        # Add changes and commit
        git add .
        if git diff-index --quiet HEAD --; then
            echo "No changes to commit in $repo_dir."
        else
            git commit -m "$commit_message"
            [[ "$push_repo" == "yes" ]] && git push origin "$branch_name"
        fi
        
        cd ..
    done
}


# Function to create pull requests from a feature branch to a base branch for all repositories
create_pull_requests() {
    local head_branch=$1
    local base_branch=${2:-development}
    local pr_title=$3
    for repo_name in "${REPOSITORIES[@]}"; do
        echo "Creating pull request for repository: $repo_name, from $head_branch to $base_branch"
        if gh pr create --repo "$GITHUB_ORG/$repo_name" --head "$head_branch" --base "$base_branch" --title "$pr_title" --fill; then
            echo "Pull request created for $repo_name"
        else
            echo "Failed to create pull request for $repo_name"
        fi
    done
}
# Function to push all repositories
push-repo() {
    for repo_name in "${REPOSITORIES[@]}"; do
        echo "Processing repository: $repo_name"
        
        local repo_path="./${repo_name}"
        if [ -d "$repo_path" ] && [ "$(ls -A "$repo_path")" ]; then
            cd "$repo_path" || exit
            
            # Initialize Git repo if not already and set the remote origin
            if [ ! -d ".git" ]; then
                echo "Initializing new Git repository for $repo_name..."
                git init
                git remote add origin "https://github.com/$GITHUB_ORG/$repo_name.git"
            elif ! git remote get-url origin > /dev/null 2>&1; then
                git remote add origin "https://github.com/$GITHUB_ORG/$repo_name.git"
            fi
            
            # Ensure you're on the default branch and it exists locally
            default_branch=$(git remote show origin | grep 'HEAD branch' | cut -d' ' -f5)
            if [ -z "$(git branch --list $default_branch)" ]; then
                git checkout -b "$default_branch"
            else
                git checkout "$default_branch"
            fi
            
            # Add, commit, and push changes
            git add .
            git commit -m "Initial commit" --allow-empty
            # Explicitly push the default branch to ensure it's created on GitHub
            git push -u origin "$default_branch"
            
            cd ..
        else
            echo "Directory for $repo_name does not exist or is empty. Skipping..."
        fi
    done
}
# Function to display help message based on the command
display_help() {
    local command=$1
    case "$command" in
        clone_repositories)
            echo "Usage: ./script.sh --command clone_repositories"
            echo "Description: Clones all repositories listed in the specified repository list file."
            ;;
        create_feature_branch)
            echo "Usage: ./script.sh --command create_feature_branch --branch-name <branch>"
            echo "Description: Creates a new feature branch in all repositories."
            ;;
        create_new_repository)
            echo "Usage: ./script.sh --command create_new_repository --repo-name <name> --visibility <visibility>"
            echo "Description: Creates a new repository on GitHub and initializes it with a .NET microservice template."
            ;;
        push_existing_repositories)
            echo "Usage: ./script.sh --command push_existing_repositories"
            echo "Description: Pushes all existing repositories to GitHub, creating the repository if it doesn't exist."
            ;;
        commit_feature)
            echo "Usage: ./script.sh --command commit_feature --branch-name <branch> --commit-message <message> --push-repo <yes|no>"
            echo "Description: Commits changes in a feature branch for all repositories with an option to push."
            ;;
        create_pull_requests)
            echo "Usage: ./script.sh --command create_pull_requests --head-branch <branch> --base-branch <branch> --pr-title <title>"
            echo "Description: Creates pull requests from a feature branch to a base branch for all repositories."
            ;;
        commit_and_push_repositories)
            echo "Usage: ./script.sh --command commit_and_push_repositories --commit-message <message> --push-repo <yes|no>"
            echo "Description: Commits and optionally pushes changes for all repositories."
            ;;
        stage_all_changes)
            echo "Usage: ./script.sh --command stage_all_changes"
            echo "Description: Stages all changes in all repositories listed in the repository list file."
            ;;
        *)
            echo "Invalid or unrecognized command for help: $command"
            exit 1
            ;;
    esac
}
# Parse command-line options for specific commands
command=""
while [[ $# -gt 0 ]]; do
    case "$1" in
        --command|-c)
            command=$2
            shift 2
            ;;
        --repo-name|-n)
            repo_name=$2
            shift 2
            ;;
        --visibility|-v)
            visibility=$2
            shift 2
            ;;
        --branch-name|-b)
            branch_name=$2
            shift 2
            ;;
        --commit-message|-m)
            commit_message=$2
            shift 2
            ;;
        --push-repo|-p)
            push_repo=$2
            shift 2
            ;;
        --repo-path|-r)
            repo_path=$2
            shift 2
            ;;
        --head-branch|-h)
            head_branch=$2
            shift 2
            ;;
        --base-branch|b)
            base_branch=$2
            shift 2
            ;;
        --pr-title|-t)
            pr_title=$2
            shift 2
            ;;
        --help|-h)
            display_help "$command"
            exit 0
            ;;
        *)
            echo "Unknown option or command: $1"
            exit 1
            ;;
    esac
done

# Execute the appropriate function based on the parsed command
case "$command" in
    clone_repositories)
        clone_repositories
        ;;
    create_feature_branch)
        create_feature_branch "$branch_name"
        ;;
    create_new_repository)
        create_new_repository "$repo_name" "$visibility"
        ;;
    push_repo)
        push_existing_repositories
        ;;
    commit_feature)
        commit_feature "$branch_name" "$commit_message" "$push_repo"
        ;;
    create_pull_requests)
        create_pull_requests "$head_branch" "$base_branch" "$pr_title"
        ;;
    commit_and_push_repositories)
        commit_and_push_repositories "$commit_message" "$push_repo"
        ;;
    stage_all_changes)
        stage_all_changes
        ;;
    *)
        echo "Invalid or missing command: $command"
        exit 1
        ;;
esac
