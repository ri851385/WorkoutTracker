using Microsoft.JSInterop;
using WorkoutTracker.Models;
using WorkoutTracker.Services;

namespace WorkoutTracker.State;

public class WorkoutSessionState(LocalStorageService localStorage, WorkoutHistoryService historyService, IJSRuntime jsRuntime)
{
    private const string CurrentSessionKey = "workout-tracker.current-session";
    private const decimal MaxWeight = 999m;
    private const int MaxReps = 999;

    private bool _isInitialized;

    public WorkoutSession? CurrentSession { get; private set; }

    public event Action? OnChange;

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;
        CurrentSession = await localStorage.GetItemAsync<WorkoutSession>(CurrentSessionKey);
    }

    public async Task StartNewSessionAsync()
    {
        if (CurrentSession is not null)
        {
            return;
        }

        var now = await GetLocalNowAsync();
        CurrentSession = new WorkoutSession
        {
            Date = now,
            Name = $"{now:yyyy年M月d日}のトレーニング",
        };
        await SaveCurrentAsync();
    }

    private async Task<DateTime> GetLocalNowAsync()
    {
        try
        {
            var iso = await jsRuntime.InvokeAsync<string>("workoutTrackerGetLocalNow");
            return DateTimeOffset.Parse(iso).DateTime;
        }
        catch (JSException)
        {
            return DateTime.Now;
        }
    }

    public async Task UpdateSessionNameAsync(string name)
    {
        if (CurrentSession is null)
        {
            return;
        }

        var trimmedName = name.Trim();
        CurrentSession.Name = string.IsNullOrEmpty(trimmedName) ? CurrentSession.Name : trimmedName;
        await SaveCurrentAsync();
    }

    public async Task AddExerciseAsync(string exerciseName)
    {
        if (CurrentSession is null)
        {
            return;
        }

        if (CurrentSession.Exercises.Any(e => e.ExerciseName == exerciseName))
        {
            return;
        }

        CurrentSession.Exercises.Add(new WorkoutExerciseEntry
        {
            ExerciseName = exerciseName,
            Sets = [new SetEntry { Weight = 0, Reps = 1 }],
        });
        await SaveCurrentAsync();
    }

    public async Task RemoveExerciseAsync(int exerciseIndex)
    {
        if (CurrentSession is null || !IsValidExerciseIndex(exerciseIndex))
        {
            return;
        }

        CurrentSession.Exercises.RemoveAt(exerciseIndex);
        await SaveCurrentAsync();
    }

    public async Task AddSetAsync(int exerciseIndex)
    {
        if (CurrentSession is null || !IsValidExerciseIndex(exerciseIndex))
        {
            return;
        }

        var sets = CurrentSession.Exercises[exerciseIndex].Sets;
        var lastSet = sets.Count > 0 ? sets[^1] : null;
        sets.Add(new SetEntry
        {
            Weight = lastSet?.Weight ?? 0,
            Reps = lastSet?.Reps ?? 1,
        });
        await SaveCurrentAsync();
    }

    public async Task RemoveSetAsync(int exerciseIndex, int setIndex)
    {
        if (CurrentSession is null || !IsValidExerciseIndex(exerciseIndex))
        {
            return;
        }

        var sets = CurrentSession.Exercises[exerciseIndex].Sets;
        if (setIndex < 0 || setIndex >= sets.Count)
        {
            return;
        }

        sets.RemoveAt(setIndex);
        await SaveCurrentAsync();
    }

    public async Task UpdateSetAsync(int exerciseIndex, int setIndex, decimal weight, int reps)
    {
        if (CurrentSession is null || !IsValidExerciseIndex(exerciseIndex))
        {
            return;
        }

        var sets = CurrentSession.Exercises[exerciseIndex].Sets;
        if (setIndex < 0 || setIndex >= sets.Count)
        {
            return;
        }

        sets[setIndex].Weight = Math.Clamp(weight, 0m, MaxWeight);
        sets[setIndex].Reps = Math.Clamp(reps, 1, MaxReps);
        await SaveCurrentAsync();
    }

    public async Task<bool> CompleteCurrentSessionAsync()
    {
        if (CurrentSession is null || CurrentSession.Exercises.Count == 0 ||
            CurrentSession.Exercises.Any(e => e.Sets.Count == 0))
        {
            return false;
        }

        CurrentSession.IsCompleted = true;
        await historyService.AddAsync(CurrentSession);

        CurrentSession = null;
        await localStorage.RemoveItemAsync(CurrentSessionKey);
        NotifyChange();
        return true;
    }

    private bool IsValidExerciseIndex(int exerciseIndex)
    {
        return CurrentSession is not null && exerciseIndex >= 0 && exerciseIndex < CurrentSession.Exercises.Count;
    }

    private async Task SaveCurrentAsync()
    {
        if (CurrentSession is not null)
        {
            await localStorage.SetItemAsync(CurrentSessionKey, CurrentSession);
        }

        NotifyChange();
    }

    private void NotifyChange() => OnChange?.Invoke();
}
