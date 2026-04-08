# Torisho.Importer

CLI project to offline data import tasks.

## Configuration

To keep the execution simple and straightforward, file paths and environment variables are configured directly in the code. 

Before running the tools, open `Program.cs` and update the following variables with your local paths:
- `kanjidicDir`: Path to your extracted KANJIDIC directory.
- `appSettingsPath`: Path to the API's `appsettings.Development.json` (used to read the `DefaultConnection` string).

## Run

Execute the following commands from the repository root (where the `.sln` file is located):

### Import KANJIDIC
Imports Kanji definitions, readings, and metadata.
```bash
dotnet run --project src/Tools/Torisho.Importer -- kanjidic
```

### Import JMdict
Imports Japanese vocabulary, meanings, and grammatical metadata.
```bash
dotnet run --project src/Tools/Torisho.Importer -- jmdict
```

### Extend For Crawl Data
To add new import pipelines (for chapters, lessons, vocabulary, grammar), add a new else if (command == "your_command") block in Program.cs and implement a importer service under the Infrastructure layer.