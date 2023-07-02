using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace SharpZip.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class HomeController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    public HomeController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }
    [HttpGet]
    public IActionResult ZipDirectory()
    {
        System.IO.Compression.ZipFile.CreateFromDirectory(@"D:\test", Path.Combine(_environment.WebRootPath, "myZip.zip"));

        return Ok();
    }

    [HttpGet]
    public IActionResult UnzipDirectory()
    {
        System.IO.Compression.ZipFile.ExtractToDirectory
            (Path.Combine(_environment.WebRootPath, "myZip.zip"), Path.Combine(_environment.WebRootPath, "other"), true);

        return Ok();
    }

    [HttpGet]
    public IActionResult ReadZipFiles()
    {
        using var zipFile = ZipFile.OpenRead(_environment.WebRootPath + "/myZip.zip");
        var counter = 0;
        var x = new List<string>();

        foreach (var entry in zipFile.Entries)
            x.Add($"{++counter,3}: {entry.Name}");

        return Ok(x);
    }

    [HttpGet]
    public IActionResult ExtractFilesFromZip()
    {
        using var zipFile = ZipFile.OpenRead(_environment.WebRootPath + "/myZip.zip");
        var rootFolder = _environment.WebRootPath;
        foreach (var entry in zipFile.Entries)
        {
            var wholePath = Path.Combine(
                rootFolder,
                Path.GetDirectoryName(entry.FullName) ?? string.Empty);
            if (!Directory.Exists(wholePath))
                Directory.CreateDirectory(wholePath);
            if (!string.IsNullOrEmpty(entry.Name))
            {
                var wholeFileName = Path.Combine(
                    wholePath,
                    Path.GetFileName(entry.FullName));
                entry.ExtractToFile(wholeFileName, true);
            }
        }

        return Ok();
    }

    [HttpGet]
    public IActionResult ExtractFilesFromZipWithOpenMethod()
    {
        using var zipFile = ZipFile.OpenRead(_environment.WebRootPath + "/myZip.zip");
        foreach (var entry in zipFile.Entries)
        {
            if (!string.IsNullOrEmpty(entry.Name))
            {
                using (var stream = entry.Open())
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    var bytes = memoryStream.ToArray();
                    var base64 = Convert.ToBase64String(bytes);
                    Console.WriteLine($"{entry.FullName} => {base64}");
                }
            }
        }

        return Ok();
    }

    [HttpGet]
    public IActionResult CreateZipFiles()
    {
        var folder = new DirectoryInfo(".");
        FileInfo[] files = folder.GetFiles("*.*", SearchOption.AllDirectories);
        using var archive = ZipFile.Open(@"..\parent.zip", ZipArchiveMode.Create);
        foreach (var file in files)
        {
            archive.CreateEntryFromFile(
                file.FullName,
                Path.GetRelativePath(folder.FullName, file.FullName)
            );
        }

        return Ok();
    }

    [HttpGet]
    public IActionResult CreateaZipFileWithStream()
    {
        var helloText = "Hello world!";

        using var archive = ZipFile.Open(@"..\test.zip", ZipArchiveMode.Create);

        var entry = archive.CreateEntry("hello.txt", CompressionLevel.SmallestSize);

        using (Stream st = entry.Open())
        using (StreamWriter writerManifest = new StreamWriter(st))
            writerManifest.WriteLine(helloText);
        return Ok();
    }

    [HttpGet]
    public IActionResult DeleteEntriesFromZipFiles()
    {
        using var zipFile = ZipFile.Open(_environment.WebRootPath + "/myZip.zip", ZipArchiveMode.Update);

        var txtFiles = zipFile.Entries.Where(e => e.Name == "1.txt").ToList();

        for (int i = txtFiles.Count - 1; i >= 0; --i)
            txtFiles[i].Delete();

        return Ok();
    }
}
