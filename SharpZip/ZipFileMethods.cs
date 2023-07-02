using System.IO.Compression;

namespace SharpZip;

public static class ZipFileMethods
{
    public static void ZipContentsOfDirectory(string sourceDir,string zipName)
    {
        System.IO.Compression.ZipFile.CreateFromDirectory(sourceDir, zipName);
    }
    
    public static void UnzipContentsOfDirectory(string destination,string zipName)
    {
        System.IO.Compression.ZipFile.ExtractToDirectory(zipName,destination);

    }
}
