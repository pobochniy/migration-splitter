using System.IO.Compression;
using System.Text;

namespace MigrationSplitter;

public static class Splitter
{
    private const string ScriptTemplate = @"
        use {0};

        insert into orders_checkinfo (`UUId`,
              OrderId, OrderUUId, CheckType,
              CheckId, CashBoxSessionId, CashBoxSessionUUId,
              CheckPrintDateTime, TotalPrice, CreatedDateTime,
              CreatedDateTimeUTC, CreatedByUserId, CreatedByUserUUId,
              PaymentType, IsActual, IsLast, IsReprint,
              IsLastSaleCheck, IsLastRefundCheck, FiscalizedCheckData,
              CheckSnapshot, ReceiptIdentifier, HasPrinted)
        select archive.`UUId`,
               archive.`OrderId`,
               archive.`OrderUUId`,
               archive.`CheckType`,
               archive.`CheckId`,
               archive.`CashBoxSessionId`,
               archive.`CashBoxSessionUUId`,
               archive.`CheckPrintDateTime`,
               archive.`TotalPrice`,
               archive.`CreatedDateTime`,
               archive.`CreatedDateTimeUTC`,
               archive.`CreatedByUserId`,
               archive.`CreatedByUserUUId`,
               archive.`PaymentType`,
               archive.`IsActual`,
               archive.`IsLast`,
               archive.`IsReprint`,
               archive.`IsLastSaleCheck`,
               archive.`IsLastRefundCheck`,
               archive.`FiscalizedCheckData`,
               archive.`CheckSnapshot`,
               archive.`ReceiptIdentifier`,
               archive.`HasPrinted`
        from orders_checkinfo_archive archive
        left join orders_checkinfo origin on archive.UUId = origin.UUId
        where archive.CashBoxSessionId >= {1}
          and archive.CashBoxSessionId < {2}
          and origin.UUId is null;
    ";
    
    public static void GenerateFiles(string country, int maxValue, int limit = 100)
    {
        var interval = 0;
        using var memoryStream = new MemoryStream();
        using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            while (interval <= maxValue)
            {
                var script = String.Format(ScriptTemplate, country, interval, interval + limit);
                var demoFile = zipArchive.CreateEntry($"{interval}.sql");

                using (var entryStream = demoFile.Open())
                {
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        streamWriter.Write(script);
                    }
                }

                interval += limit;
            }
        }

        var zipName = string.Format(Settings.OutputZipPathTemplate, country);
        File.WriteAllBytes(zipName, memoryStream.ToArray());
    }
}