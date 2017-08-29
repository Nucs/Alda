# Alda
Autocad Lisp Debug Assistant (ALDA) allows fast autocad lisp development by automatic .lsp reloading with referencing support.

### Features
  - Automatically reload selected directory/indvidual lisp files by detecting changes.
  - Reference other .lsp files and reload the entire reference hierarchy.

![](http://i.imgur.com/fYmZ3Fg.png) 
### Requirements
* [.Net Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=30653)
* Autocad 2017+ (for lower versions, please post and issue).

### Loading
1. Download a release or compile it by yourself and then run the following command:
    ```lsp
    (command "netload" "Alda.dll" )
    ```
    or pick the dll manually by calling `netload`.
2. After loading the .Dll succesfully, you can now call `lspdbg` which will start the GUI.

### Automatic load on startup
Create a lisp file, fill the following and add it to the [Startup Suite](https://knowledge.autodesk.com/support/autocad/learn-explore/caas/CloudHelp/cloudhelp/2016/ENU/AutoCAD-Core/files/GUID-B38F610B-51FB-4938-BDEC-A0A737F5DB6C-htm.html).
```
(command "netload" "Alda.dll" )
(command "lspdbg" )
```
Note: There are settings to quicken the startup procedure such as "Load all lisp on startup" or "On debugger startup, start listening automatically".

### Referencing
Lets say we have five files which are referenced in the following order:
![](http://i.imgur.com/vBnWPms.png)
In words; A references to C and B; C references to D and E; B references to E.
If it's still hard to undestand, replace the work "references" to "uses".
Now what happens when we make changes in A file, only A will reload.
When we change C then C and A will reload.
When we change E then C, B and A will reload (in that order).

##### How to add a reference?
Syntax:
```
;#	C:/path/to/file.lsp
;#	C:/path/to/filetwo.lsp
or
;#	insidethesamefolder.lsp
```
It can be placed anywhere in the file, but for readability - keep it on top.
Also this causes any comment that starts by `;#` to be a reference marker - so keep in mind to remove any comments with that format.
##### Referencing Requirement
In order for the reloader to identify all references and reload accordingly, they must be added to watch list.
If the .lsp file is listed, it will continue reloading what any files it knows about.
### Upcoming Features
* Lisp commands API to interact with the debugger from commandline/lisp files.
* Hard Reloading Option - will require to have all references watched in order to reload.

### License
----

MIT

