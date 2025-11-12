using Labb_3_Quiz_Configurator.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace Labb_3_Quiz_Configurator.Data;

public static class QuestionPackStorage
{
    private static readonly string FolderPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "QuizConfigurator");

    private static readonly string FilePath = Path.Combine(FolderPath, "questionpacks.json");

    public static async Task SaveAsync(List<QuestionPack> packs)
    {
        if (!Directory.Exists(FolderPath))
            Directory.CreateDirectory(FolderPath);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        var json = JsonSerializer.Serialize(packs, options);
        try
        {
            await File.WriteAllTextAsync(FilePath, json).ConfigureAwait(false);
            System.Diagnostics.Debug.WriteLine($"[QuestionPackStorage] Wrote {packs.Count} packs to {FilePath}");
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("[QuestionPackStorage] Save failed: " + ex);
            throw;
        }
    }

    public static async Task<List<QuestionPack>> LoadAsync()
    {
        if (!Directory.Exists(FolderPath))
            Directory.CreateDirectory(FolderPath);

        if (!File.Exists(FilePath))
        {
            await File.WriteAllTextAsync(FilePath, "[]").ConfigureAwait(false);
            return new List<QuestionPack>();
        }

        var json = await File.ReadAllTextAsync(FilePath).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(json))
            return new List<QuestionPack>();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<List<QuestionPack>>(json, options) ?? new List<QuestionPack>();
    }
}
