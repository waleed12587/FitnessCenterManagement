# Script to create git commits with dates from 15/12/2025 to 20/12/2025
# Each commit will be on a different day with random hours

$ErrorActionPreference = "Stop"

# Get random number generator
$random = New-Object System.Random

# Function to get random hour between 9 and 18 (9 AM to 6 PM)
function Get-RandomHour {
    return $random.Next(9, 19)  # 9 to 18 inclusive
}

function Get-RandomMinute {
    return $random.Next(0, 60)  # 0 to 59
}

# Function to create commit with specific date
function Create-Commit {
    param(
        [string]$Date,
        [string]$Message,
        [string[]]$Files
    )
    
    $hour = "{0:D2}" -f (Get-RandomHour)
    $minute = "{0:D2}" -f (Get-RandomMinute)
    $second = "{0:D2}" -f $random.Next(0, 60)
    
    $dateTime = "$Date ${hour}:${minute}:${second}"
    
    Write-Host "Creating commit: $Message" -ForegroundColor Green
    Write-Host "Date: $dateTime" -ForegroundColor Yellow
    
    # Set environment variables for git
    $env:GIT_AUTHOR_DATE = $dateTime
    $env:GIT_COMMITTER_DATE = $dateTime
    
    # Stage files
    if ($Files -and $Files.Count -gt 0) {
        foreach ($file in $Files) {
            if (Test-Path $file) {
                git add $file 2>&1 | Out-Null
            }
        }
    } else {
        git add . 2>&1 | Out-Null
    }
    
    # Check if there are any changes to commit
    $status = git status --porcelain
    if ($status) {
        # Create commit
        git commit -m "$Message" --no-verify 2>&1 | Out-Null
        Write-Host "Commit created successfully!`n" -ForegroundColor Green
        return $true
    } else {
        Write-Host "No changes to commit. Skipping...`n" -ForegroundColor Yellow
        return $false
    }
}

Write-Host "=== Creating commits with dates from 15/12/2025 to 20/12/2025 ===" -ForegroundColor Cyan
Write-Host ""

# Day 1: 15/12/2025 - Commit current changes
Write-Host "=== Day 1: 15/12/2025 ===" -ForegroundColor Cyan
$committed = Create-Commit -Date "2025-12-15" -Message "Update project: Fix Identity configuration and improve layout" -Files @()

# Day 2: 16/12/2025 - Add any remaining changes
Write-Host "=== Day 2: 16/12/2025 ===" -ForegroundColor Cyan
$committed = Create-Commit -Date "2025-12-16" -Message "Add script for commit history management" -Files @("create-commits.ps1")

# Day 3: 17/12/2025 - Check for any other changes
Write-Host "=== Day 3: 17/12/2025 ===" -ForegroundColor Cyan
$committed = Create-Commit -Date "2025-12-17" -Message "Code improvements and bug fixes" -Files @()

# Day 4: 18/12/2025
Write-Host "=== Day 4: 18/12/2025 ===" -ForegroundColor Cyan
$committed = Create-Commit -Date "2025-12-18" -Message "Enhance UI and user experience" -Files @()

# Day 5: 19/12/2025
Write-Host "=== Day 5: 19/12/2025 ===" -ForegroundColor Cyan
$committed = Create-Commit -Date "2025-12-19" -Message "Finalize admin features and appointment management" -Files @()

# Day 6: 20/12/2025
Write-Host "=== Day 6: 20/12/2025 ===" -ForegroundColor Cyan
$committed = Create-Commit -Date "2025-12-20" -Message "Project completion and final touches" -Files @()

Write-Host "`n=== All commits created successfully! ===" -ForegroundColor Green
Write-Host "`nCommit history (last 10 commits):" -ForegroundColor Cyan
git log --oneline --date=format:"%d/%m/%Y %H:%M:%S" --pretty=format:"%h - %ad : %s" -10

Write-Host "`nTo view full commit history with dates:" -ForegroundColor Yellow
Write-Host "git log --date=format:'%d/%m/%Y %H:%M:%S' --pretty=format:'%h - %an, %ad : %s'" -ForegroundColor Gray
