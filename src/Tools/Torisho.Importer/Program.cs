using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Torisho.Application.Interfaces.Dictionary;
using Torisho.Infrastructure;
using Torisho.Infrastructure.Services.Dictionary;

var appSettingsPath = "src/Presentation/Torisho.API/appsettings.Development.json";

var kanjidicDir = @"D:\PBL5\Torisho-BE\data\KANJIDIC_english"; 
var jmdictFile = @"D:\PBL5\Torisho-BE\data\JMdict_e.json";     
var rebuildEntryLinks = true;

// get command from args, default "kanjidic" if not provided
var command = args.Length > 0 ? args[0].ToLowerInvariant() : "kanjidic";

var connectionString = GetConnectionString(appSettingsPath);
if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.Error.WriteLine($"[LỖI] Không tìm thấy chuỗi kết nối (DefaultConnection) tại: {appSettingsPath}");
    return 1;
}

var dbOptions = new DbContextOptionsBuilder<DataContext>()
    .UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions =>
        {
            mySqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
            mySqlOptions.CommandTimeout(60);
            mySqlOptions.MigrationsAssembly("Torisho.Infrastructure");
        })
    .Options;

await using var context = new DataContext(dbOptions);

try
{
    if (command == "kanjidic")
    {
        Console.WriteLine($"[*] Đang bắt đầu import KANJIDIC từ: {kanjidicDir}");
        var service = new KanjidicImportService(context);
        var result = await service.ImportAsync(kanjidicDir, rebuildEntryLinks);
        
        Console.WriteLine("[v] Import KANJIDIC hoàn tất.");
        Console.WriteLine($"    -> Đã xử lý: {result.ProcessedFiles} | Tạo mới: {result.Created} | Cập nhật: {result.Updated} | Bỏ qua: {result.Skipped}");
    }
    else if (command == "jmdict")
    {
        Console.WriteLine($"[*] Đang bắt đầu import JMdict từ: {jmdictFile}");
        if (!File.Exists(jmdictFile)) throw new FileNotFoundException($"Không tìm thấy file: {jmdictFile}");

        await using var stream = File.OpenRead(jmdictFile);
        IJmdictImportService service = new JmdictImportService(context);
        var result = await service.ImportAsync(stream);

        Console.WriteLine("[v] Import JMdict hoàn tất.");
        Console.WriteLine($"    -> Tạo mới: {result.Created} | Cập nhật: {result.Updated} | Bỏ qua: {result.Skipped}");
    }
    else
    {
        Console.Error.WriteLine($"[!] Lệnh không hợp lệ: '{command}'. Hãy dùng 'kanjidic' hoặc 'jmdict'.");
        return 1;
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine($"[LỖI] Quá trình import thất bại: {ex.Message}");
    return 1;
}

return 0;

static string? GetConnectionString(string filePath)
{
    if (!File.Exists(filePath)) return null;
    try
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(filePath));
        return doc.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("DefaultConnection")
            .GetString();
    }
    catch
    {
        return null;
    }
}