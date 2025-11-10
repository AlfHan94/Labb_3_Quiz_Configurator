using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Labb_3_Quiz_Configurator.Models;

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

            var json = JsonSerializer.Serialize(packs, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(FilePath, json);
        }

        public static async Task<List<QuestionPack>> LoadAsync()
        {
            if (!File.Exists(FilePath))
                return new List<QuestionPack>();

            var json = await File.ReadAllTextAsync(FilePath);
            return JsonSerializer.Deserialize<List<QuestionPack>>(json) ?? new List<QuestionPack>();
        }
    }

