Editor Console Pro is a powerful replacement for Unity's editor console.

It is accessed in the Window menu under Console Pro, or by pressing Command + \

# Panel buttons
The bottom right buttons allow you to hide/show various things:
Color toggles colorized entries.  When this is on the entire line of a log will have the same color as it's filter.  For example, Errors will be red.
File toggles the file name that the Debug.Log was called from in a column.
Class toggles the class and method name that the Debug.Log was called from in a column.
Time toggles the time stamp for the Debug.Log.
Stack toggles the stack trace panel.
Src toggles source code view in the stack trace panel.  This lets you see the surrounding source code for every function call in the stack trace.  Clicking on any line in the source view will let you jump directly to it.

# Filtering by search:
Type a search string in the top right <filter> field to only show entries containing the search text.

# Custom filters:
Custom filters allow you to add more log types to the toolbar next to Errors, Warnings, Logs and to give them a custom color.  You can filter them by type and string.
Create a filter by:
Press the Prefs button to bring up preferences.
Press New Filter.
Here you can name your filter, select the log type (error, warning, log), pick what to filter from (file name, class/method containing the debug.log call, or log text), and a color to identify them with.

# Jumping to stack entries
Simply click on any line in the stack panel to jump to that line in the source code.  Double clicking a log entry jumps to the top stack entry as normal.

# Copying and Exporting
Press the Copy Stack button to put a copy of the current log and it's stack into the clipboard
Press Export and pick a file name to export a text file of the entire log.  You can use this to compare against different plays of your game.

# Support
Please EMail all support and feature requests to Support@Flyingworm.com