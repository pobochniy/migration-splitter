using System.IO.Compression;
using System.Text;

namespace MigrationSplitter;
public record MigrationInfo(string FileName, string DatabaseName, int Min, int Max);
public static class Splitter
{
    public static void GenerateFiles(string template, MigrationInfo info, int batchSize = 5000)
    {
        var min = 0;
        var count = 1;
        using var memoryStream = new MemoryStream();
        using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            while (min <= info.Max)
            {
                var script = template
                    .Replace("{database}", info.DatabaseName)
                    .Replace("{min}", min.ToString())
                    .Replace("{max}", Math.Min(min + batchSize - 1, info.Max).ToString());
                var file = zipArchive.CreateEntry($"{info.FileName}_{count.ToString().PadLeft(6, '0')}.sql");

                using (var entryStream = file.Open())
                {
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        streamWriter.Write(script);
                    }
                }

                min += batchSize;
                count++;
            }
        }

        var zipName = $"/Users/viktorpobochniy/proj/migrations-temp/ordercomposition_customization_{info.FileName}.zip";
        File.WriteAllBytes(zipName, memoryStream.ToArray());
    }
}