using System.IO.Compression;

namespace MigrationRowGenerator;
public static class Splitter
{
    public static void GenerateFiles(string databaseName, string fileName, List<string> insertRows, int batchSize = 5000)
    {
        var count = 1;
        using var memoryStream = new MemoryStream();
        using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            var chunks = insertRows.Chunk(batchSize);

            foreach (var chunk in chunks)
            {
                var file = zipArchive.CreateEntry($"{fileName}_{count.ToString().PadLeft(6, '0')}.sql");

                using (var entryStream = file.Open())
                {
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        streamWriter.WriteLine($"use {databaseName};");

                        foreach (var row in chunk)
                        {
                            streamWriter.WriteLine(row);
                        }
                    }
                }
                count++;
            }
        }

        var zipName = $"/Users/viktorpobochniy/proj/migrations-temp/ordercomposition_customization_{fileName}.zip";
        File.WriteAllBytes(zipName, memoryStream.ToArray());
    }
}