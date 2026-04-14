using System.Diagnostics;
using System.Net.Http;

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
var nativePatch = new Process
{
    StartInfo = new ProcessStartInfo
    {
        FileName = "git",
        Arguments = "apply ../FNAWasm.patch",
        WorkingDirectory = "FNAWasm",
    }
};
nativePatch.Start();
nativePatch.WaitForExit();
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

