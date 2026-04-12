using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;

static void PatchFile(string filePath, string oldText, string newText)
{
    var content = File.ReadAllText(filePath);
    if (!content.Contains(oldText))
    {
        Console.WriteLine($"Pattern not found in {Path.GetFileName(filePath)} (skipped)");
        return;
    }

    content = content.Replace(oldText, newText);
    File.WriteAllText(filePath, content);
    Console.WriteLine($"Patched {Path.GetFileName(filePath)}");
}

// get args flags for the script
bool doServe = false;
bool doClean = false;
foreach (String arg in args)
{
    switch (arg)
    {
        case "serve":
            doServe = true;
            break;
        case "clean":
            doClean = true;
            break;
    }
}

if (doClean)
{
    Console.WriteLine("Cleaning project...");
    var cleanProcess = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "clean -c Release -v d",
            WorkingDirectory = "FNAWasmRunner",
        }
    };
    cleanProcess.Start();
    cleanProcess.WaitForExit();
    Console.WriteLine("Finished cleaning project");
}

// Publish the project to get the latest framework files
var publishProcess = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = "dotnet",
        Arguments = "publish -c Release -v d",
        WorkingDirectory = "FNAWasmRunner",
    }
};

publishProcess.Start();
publishProcess.WaitForExit();

Console.WriteLine("Finished publishing project");
Console.WriteLine("Now patching framework files...");
string frameworkDir = Path.Combine("FNAWasmRunner", "bin", "Release", "net10.0", "publish", "wwwroot", "_framework");
Console.WriteLine("1) dotnet.runtime.*.js patch");
Console.WriteLine("fixes mono init with -sWASMFS enabled");
var runtimeFile = Directory.GetFiles(frameworkDir, "dotnet.runtime.*.js").FirstOrDefault();
if (runtimeFile is not null)
{
    PatchFile(
        runtimeFile,
        "FS_createPath(\"/\",\"usr/share\",!0,!0)",
        "FS_createPath(\"/usr\",\"share\",!0,!0)"
    );
}
Console.WriteLine("2) dotnet.native.*.js patch");
Console.WriteLine("automatically forces transfer of canvas matching selector `.canvas` (class canvas) to deputy thread (c# managed main thread)");
var nativeFile = Directory.GetFiles(frameworkDir, "dotnet.native.*.js").FirstOrDefault();
if (nativeFile is not null)
{
    PatchFile(
        nativeFile,
        "var offscreenCanvases={};",
        "var offscreenCanvases={};if(globalThis.window&&!window.TRANSFERRED_CANVAS){transferredCanvasNames=[\".canvas\"];window.TRANSFERRED_CANVAS=true;}"
    );
}

if (doServe)
{
    Console.WriteLine("Starting local server...");
    var serveProcess = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "python3",
            Arguments = "tools/serve.py",
            WorkingDirectory = "FNAWasmRunner",
        }
    };
    serveProcess.Start();
    serveProcess.WaitForExit();
}

