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

    public async Task<List<ExerciseCategory>> GetCategorizedExercisesAsync()
    {
        var categories = PresetCategories
            .Select(c => new ExerciseCategory { Name = c.Name, Exercises = [.. c.Exercises] })
            .ToList();

        var customExercises = await GetCustomExercisesAsync();
        if (customExercises.Count > 0)
        {
            categories.Add(new ExerciseCategory { Name = OtherCategoryName, Exercises = customExercises });
        }

        return categories;
    }

    public async Task<List<string>> GetCustomExercisesAsync()
    {
        return await localStorage.GetItemAsync<List<string>>(CustomExercisesKey) ?? [];
    }

    public async Task AddCustomExerciseAsync(string name)
    {
        var trimmedName = name.Trim();
        if (string.IsNullOrEmpty(trimmedName))
        {
            return;
        }

        var allExisting = PresetCategories.SelectMany(c => c.Exercises);
        var customExercises = await GetCustomExercisesAsync();
        if (allExisting.Contains(trimmedName) || customExercises.Contains(trimmedName))
        {
            return;
        }

        customExercises.Add(trimmedName);
        await localStorage.SetItemAsync(CustomExercisesKey, customExercises);
    }
}
