# Torisho.Importer

CLI project to offline data import tasks.

## Configuration

The importer now auto-detects the repository root from the current working directory and resolves:
- `data/KANJIDIC_english`
- `data/jmdict-eng-common-3.6.1.json`
- `src/Presentation/Torisho.API/appsettings.Development.json`

This means you can run it from either the repository root or directly inside `src/Tools/Torisho.Importer`.

## Run

Run from the repository root:

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

Or run directly inside `src/Tools/Torisho.Importer`:

```bash
dotnet run -- kanjidic
dotnet run -- jmdict
```

### Extend For Crawl Data
To add new import pipelines (for chapters, lessons, vocabulary, grammar), add a new else if (command == "your_command") block in Program.cs and implement a importer service under the Infrastructure layer.
