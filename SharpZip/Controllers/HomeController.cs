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
        var destination = Path.Combine(_environment.WebRootPath, "myZip.zip");
        ZipFileMethods.ZipContentsOfDirectory(@"D:\test", destination);

        return Ok();
    }
    
    [HttpGet]
    public IActionResult UnZipDirectory()
    {
        var source = Path.Combine(_environment.WebRootPath, "other");
        ZipFileMethods.UnzipContentsOfDirectory(source,Path.Combine(_environment.WebRootPath,"myZip.zip"));

        return Ok();
    }
}
