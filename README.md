REPL console app for Visual Studio for Mac

Usage:

    mono AutoTest.Repl.exe --file=path/to/VisualStudio.exe

    mono AutoTest.Repl.exe --file=path/to/VisualStudio.exe --attach

AutoTestClientSession available as 'app' variable.

    app.WaitForElement(c => c.Marked("Manage NuGet Packages"))

```
Mono C# Shell, type "help;" for help

Enter statements below.
csharp> app.WaitForElement(c => c.Marked("Manage NuGet Packages"))
{ NSObject: Type: MonoDevelop.MacIntegration.ExtendedMacTitleBarDialogBackend }
```
