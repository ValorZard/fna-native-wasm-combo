using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net;

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

static async Task CopyBinaries()
{
    Console.WriteLine("Now copying files from FNA-WASM-Build...");
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

// Copy the latest binaries from the releases of FNA-WASM-Build
if (doClean || !Directory.Exists("FNAWasmRunner\\statics") || !Directory.GetFiles("FNAWasmRunner\\statics").Any())
{
    await CopyBinaries();
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
    var port = 5000;

    static string GetContentType(string extension) => extension.ToLowerInvariant() switch
    {
        ".html" => "text/html; charset=utf-8",
        ".htm" => "text/html; charset=utf-8",
        ".js" => "application/javascript",
        ".mjs" => "application/javascript",
        ".wasm" => "application/wasm",
        ".css" => "text/css; charset=utf-8",
        ".json" => "application/json",
        ".png" => "image/png",
        ".jpg" => "image/jpeg",
        ".jpeg" => "image/jpeg",
        ".gif" => "image/gif",
        ".svg" => "image/svg+xml",
        ".ico" => "image/x-icon",
        _ => "application/octet-stream"
    };

    static string? ResolvePath(string urlPath)
    {
        var path = WebUtility.UrlDecode(urlPath);
        if (string.IsNullOrEmpty(path))
        {
            path = "/";
        }

        // default to index.html for directory paths
        if (path.EndsWith('/'))
        {
            path += "index.html";
        }

        // special logic for framework files to serve from the publish output instead of wwwroot
        var normalized = path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);

        if (path.StartsWith("/_framework", StringComparison.Ordinal))
        {
            var suffix = path["/_framework".Length..].TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine("FNAWasmRunner", "bin", "Release", "net10.0", "publish", "wwwroot", "_framework", suffix);
        }

        // serve content files from the Content directory (should be copied to output on build)
        if (path.StartsWith("/Content", StringComparison.Ordinal))
        {
            var suffix = path["/Content".Length..].TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine("Content", suffix);
        }

        return Path.Combine("FNAWasmRunner", "wwwroot", normalized);
    }

    using var listener = new HttpListener();
    listener.Prefixes.Add($"http://localhost:{port}/");
    listener.Start();

    Console.WriteLine($"Serving on http://localhost:{port}");
    Console.WriteLine("Press Ctrl+C to stop.");

    while (true)
    {
        var context = await listener.GetContextAsync();
        var response = context.Response;
        response.Headers["Access-Control-Allow-Origin"] = "*";
        response.Headers["Cross-Origin-Embedder-Policy"] = "require-corp";
        response.Headers["Cross-Origin-Opener-Policy"] = "same-origin";

        var filePath = ResolvePath(context.Request.Url?.AbsolutePath ?? "/");
        if (filePath is null || !File.Exists(filePath))
        {
            response.StatusCode = 404;
            response.Close();
            continue;
        }

        var extension = Path.GetExtension(filePath);
        response.ContentType = GetContentType(extension);

        await using var stream = File.OpenRead(filePath);
        response.ContentLength64 = stream.Length;
        await stream.CopyToAsync(response.OutputStream);
        response.OutputStream.Close();
    }
}

Console.WriteLine("Done! Press any key to exit...");
Console.ReadKey();