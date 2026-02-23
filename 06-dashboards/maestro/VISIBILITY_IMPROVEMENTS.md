# Visibility Improvements - Real-Time Progress Tracking

## Problem
Users couldn't tell what was happening while workflows were running - tasks appeared to hang with no feedback.

## Solution
Added comprehensive real-time visibility features:

### 1. **Task Execution Headers**
Each task now shows a clear header with:
- Task name
- Agent role
- Timeout duration
- Visual separator

```
================================================================================
🔄 [TASK] analyze_tdd_and_plans
📋 Role: Architect
⏱️  Timeout: 120 minutes
================================================================================
```

### 2. **Real-Time Progress Indicator**
Animated progress indicator showing:
- Task status (RUNNING)
- Elapsed time (updates every second)
- Animated dots for visual feedback

```
⏳ [RUNNING] analyze_tdd_and_plans ... (45s elapsed)
```

### 3. **Verbose Mode (`--verbose` or `-v`)**
When enabled, shows:
- **Prompts**: First 500 characters of the prompt being sent
- **Commands**: The actual command being executed
- **Partial Output**: Real-time output as it streams from cursor-agent
- **Error Details**: Full stdout/stderr on failures

### 4. **Streaming Output**
Changed from `exec` to `spawn` for:
- Real-time output streaming
- Better visibility into what cursor-agent is doing
- Ability to see partial results as they come in

### 5. **Completion Status**
Clear completion messages:
- ✅ Success: `✅ [COMPLETE] task_name (123.4s)`
- ❌ Failure: `❌ [FAILED] task_name (45.2s)` with error details

## Usage

### Basic Mode (Default)
```bash
npm run dev run -- --flow fwid-compliance --runner cursor
```

**Output:**
- Task headers
- Progress indicators
- Completion status
- Summary

### Verbose Mode
```bash
npm run dev run -- --flow fwid-compliance --runner cursor --verbose
# or
npm run dev run -- --flow fwid-compliance --runner cursor -v
```

**Additional Output:**
- Prompts being sent
- Commands being executed
- Real-time partial output
- Detailed error information

## Example Output

### Normal Mode
```
================================================================================
🔄 [TASK] analyze_tdd_and_plans
📋 Role: Architect
⏱️  Timeout: 120 minutes
================================================================================
⏳ [RUNNING] analyze_tdd_and_plans ... (45s elapsed)
✅ [COMPLETE] analyze_tdd_and_plans (123.4s)
```

### Verbose Mode
```
================================================================================
🔄 [TASK] analyze_tdd_and_plans
📋 Role: Architect
⏱️  Timeout: 120 minutes
================================================================================

📝 Prompt (first 500 chars):
You are an Architect agent. Your task is to analyze the TDD document...

💻 Command: cursor-agent -p --force --output-format json "You are an Architect..."

⏳ [RUNNING] analyze_tdd_and_plans ... (45s elapsed)
   {"status": "processing", "progress": 25%}
   {"status": "analyzing", "section": "Domain Entities"}
   {"status": "complete", "summary": "Analysis complete"}

✅ [COMPLETE] analyze_tdd_and_plans (123.4s)

📄 Output details (first 500 chars):
{"success": true, "summary": "TDD analysis complete", ...}
```

## Features

### Progress Tracking
- **Elapsed Time**: Updates every second
- **Visual Feedback**: Animated dots (`.`, `..`, `...`)
- **Status Updates**: Clear indication of what's happening

### Error Visibility
- **Clear Error Messages**: Shows what went wrong
- **Error Details**: In verbose mode, shows stdout/stderr
- **Timing Information**: How long the task ran before failing

### Streaming Output
- **Real-Time Updates**: See output as it's generated
- **Partial Results**: Don't wait for completion to see progress
- **Verbose Filtering**: Only shows relevant lines in verbose mode

## Technical Details

### Implementation
- **Streaming**: Uses `spawn` instead of `exec` for real-time output
- **Progress Indicator**: `setInterval` updates every second
- **Command Parsing**: Handles quoted strings properly for shell execution
- **Timeout Handling**: Properly cleans up intervals and processes

### Performance
- **Minimal Overhead**: Progress indicator uses minimal resources
- **Non-Blocking**: Streaming doesn't block execution
- **Clean Shutdown**: Proper cleanup on completion or error

## Files Modified

1. **`src/agents/CursorCliAgentRunner.ts`**
   - Added `verbose` option
   - Implemented `showProgress()` method
   - Implemented `executeWithStreaming()` method
   - Added command parsing for spawn
   - Enhanced error reporting

2. **`src/cli/commands/run.ts`**
   - Added `verbose` option to `RunOptions`
   - Passes verbose flag to `CursorCliAgentRunner`

3. **`src/cli/index.ts`**
   - Added `--verbose` / `-v` CLI flag

## Benefits

1. **Visibility**: Know what's happening in real-time
2. **Debugging**: Verbose mode helps diagnose issues
3. **Confidence**: Progress indicators show tasks aren't hung
4. **Transparency**: See exactly what prompts are being sent
5. **Feedback**: Know how long tasks are taking

## Next Steps

Try running with verbose mode to see the full output:

```bash
npm run dev run -- --flow fwid-compliance --runner cursor --verbose
```

This will show you exactly what's happening at each step!
