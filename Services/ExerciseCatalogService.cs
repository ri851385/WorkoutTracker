using System.Text.Json;
using WorkoutTracker.Models;

namespace WorkoutTracker.Services;

public class ExerciseCatalogService(LocalStorageService localStorage)
{
    private const string CustomExercisesKey = "workout-tracker.custom-exercises";
    private const string OtherCategoryName = "その他";

    private static readonly List<ExerciseCategory> PresetCategories =
    [
        new() { Name = "胸", Exercises = ["ベンチプレス", "ダンベルプレス", "インクラインベンチプレス", "ダンベルフライ", "プッシュアップ"] },
        new() { Name = "背中", Exercises = ["デッドリフト", "ラットプルダウン", "ベントオーバーロウ", "懸垂", "ワンハンドロウ"] },
        new() { Name = "脚", Exercises = ["スクワット", "レッグプレス", "レッグエクステンション", "レッグカール", "ランジ"] },
        new() { Name = "肩", Exercises = ["ショルダープレス", "サイドレイズ", "フロントレイズ", "アップライトロウ"] },
        new() { Name = "腕", Exercises = ["アームカール", "トライセプスエクステンション", "ハンマーカール", "ディップス"] },
        new() { Name = "腹", Exercises = ["クランチ", "プランク", "レッグレイズ", "アブローラー"] },
        new() { Name = "有酸素", Exercises = ["ランニング", "ウォーキング", "サイクリング", "縄跳び"] },
    ];

    public List<string> GetCategoryNames() => [.. PresetCategories.Select(c => c.Name), OtherCategoryName];

    public async Task<List<ExerciseCategory>> GetCategorizedExercisesAsync()
    {
        var customExercises = await GetCustomExercisesAsync();

        var categories = PresetCategories
            .Select(c => new ExerciseCategory
            {
                Name = c.Name,
                Exercises = [.. c.Exercises, .. customExercises.Where(e => e.Category == c.Name).Select(e => e.Name)],
            })
            .ToList();

        var otherExercises = customExercises.Where(e => e.Category == OtherCategoryName).Select(e => e.Name).ToList();
        if (otherExercises.Count > 0)
        {
            categories.Add(new ExerciseCategory { Name = OtherCategoryName, Exercises = otherExercises });
        }

        return categories;
    }

    public async Task<List<CustomExercise>> GetCustomExercisesAsync()
    {
        try
        {
            return await localStorage.GetItemAsync<List<CustomExercise>>(CustomExercisesKey) ?? [];
        }
        catch (JsonException)
        {
            // Migrate from the older format, which stored custom exercises as plain name strings.
            var legacyNames = await localStorage.GetItemAsync<List<string>>(CustomExercisesKey) ?? [];
            var migrated = legacyNames.Select(name => new CustomExercise { Name = name, Category = OtherCategoryName }).ToList();
            await localStorage.SetItemAsync(CustomExercisesKey, migrated);
            return migrated;
        }
    }

    public async Task AddCustomExerciseAsync(string name, string category)
    {
        var trimmedName = name.Trim();
        if (string.IsNullOrEmpty(trimmedName))
        {
            return;
        }

        var resolvedCategory = string.IsNullOrWhiteSpace(category) ? OtherCategoryName : category;

        var allExisting = PresetCategories.SelectMany(c => c.Exercises);
        var customExercises = await GetCustomExercisesAsync();
        if (allExisting.Contains(trimmedName) || customExercises.Any(e => e.Name == trimmedName))
        {
            return;
        }

        customExercises.Add(new CustomExercise { Name = trimmedName, Category = resolvedCategory });
        await localStorage.SetItemAsync(CustomExercisesKey, customExercises);
    }

    public async Task RemoveCustomExerciseAsync(string name)
    {
        var customExercises = await GetCustomExercisesAsync();
        var updated = customExercises.Where(e => e.Name != name).ToList();
        await localStorage.SetItemAsync(CustomExercisesKey, updated);
    }
}
