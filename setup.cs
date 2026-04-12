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

Console.WriteLine("Now copying files...");
if (Directory.Exists("FNAWasmRunner\\statics"))
{
    Directory.Delete("FNAWasmRunner\\statics", true);
}
try
{
    Directory.CreateDirectory("FNAWasmRunner\\statics");
    using var client = new HttpClient();

    string staticsRelease = "eb111fb8-7474-4f75-a1b7-848fc6293aa5"; 

    string baseUrl = $"https://github.com/r58Playz/FNA-WASM-Build/releases/download/{staticsRelease}";
    string outputDir = "FNAWasmRunner\\statics";

    string[] files =
    [
        "FAudio.a",
        "FNA3D.a",
        "libmojoshader.a",
        "SDL3.a"
    ];

    foreach (string file in files)
    {
        string sourceUrl = $"{baseUrl}/{file}";
        string destinationPath = Path.Combine(outputDir, file);

        Console.WriteLine($"Downloading {file}...");
        using HttpResponseMessage response = await client.GetAsync(sourceUrl, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using Stream source = await response.Content.ReadAsStreamAsync();
        await using FileStream destination = File.Create(destinationPath);
        await source.CopyToAsync(destination);
    }
}
catch (Exception e)
{
    Console.WriteLine($"Error copying files: {e}");
}