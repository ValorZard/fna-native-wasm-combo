using System.Diagnostics;
using System.Net.Http;

bool doClean = false;
for (int i = 0; i < args.Length; i++)
{
    if (args[i] == "clean")
    {
        doClean = true;
        break;
    }
}

if (doClean)
{
    Console.WriteLine("Cleaning up...");
    if (Directory.Exists("FNANative"))
        Directory.Delete("FNANative", true);
    if (Directory.Exists("FNAWasm"))
        Directory.Delete("FNAWasm", true);
    if (Directory.Exists("FontStashSharp"))
        Directory.Delete("FontStashSharp", true);
    Console.WriteLine("Finished cleaning up");
}

String branch = "26.04";
var nativeClone = new Process();
var wasmClone = new Process();
nativeClone.StartInfo.FileName = "git";
nativeClone.StartInfo.Arguments = $"clone https://github.com/FNA-XNA/FNA --recursive -b {branch} FNANative";
nativeClone.Start();
wasmClone.StartInfo.FileName = "git";
wasmClone.StartInfo.Arguments = $"clone https://github.com/FNA-XNA/FNA --recursive -b {branch} FNAWasm";
wasmClone.Start();
nativeClone.WaitForExit();
wasmClone.WaitForExit();
Console.WriteLine("Finished cloning FNA");
Console.WriteLine("Now applying patches...");
var wasmPatch = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = "git",
        Arguments = "apply ../FNAWasm.patch",
        WorkingDirectory = "FNAWasm",
    }
};
wasmPatch.Start();
wasmPatch.WaitForExit();
Console.WriteLine("Finished applying patches");

Console.WriteLine("Now downloading dependencies...");
Console.WriteLine("Downloading FontStashSharp...");
var fontStashClone = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = "git",
        Arguments = "clone https://github.com/FontStashSharp/FontStashSharp.git --recursive",
    }
};
fontStashClone.Start();
fontStashClone.WaitForExit();
Console.WriteLine("Finished downloading FontStashSharp");
var fontStashPatch = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = "git",
        Arguments = "apply ../FontStashSharp.patch",
        WorkingDirectory = "FontStashSharp",
    }
};
fontStashPatch.Start();
fontStashPatch.WaitForExit();
Console.WriteLine("Finished applying FontStashSharp patches");
