using System.Diagnostics;

var nativeClone = new Process();
var wasmClone = new Process();
nativeClone.StartInfo.FileName = "git";
nativeClone.StartInfo.Arguments = "clone https://github.com/FNA-XNA/FNA --recursive -b 26.04 FNANative";
nativeClone.Start();
wasmClone.StartInfo.FileName = "git";
wasmClone.StartInfo.Arguments = "clone https://github.com/FNA-XNA/FNA --recursive -b 26.04 FNAWasm";
wasmClone.Start();
nativeClone.WaitForExit();
wasmClone.WaitForExit();
Console.WriteLine("Done!");