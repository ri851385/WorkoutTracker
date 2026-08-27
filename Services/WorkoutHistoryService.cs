using WorkoutTracker.Models;

namespace WorkoutTracker.Services;

public class WorkoutHistoryService(LocalStorageService localStorage)
{
    private const string HistoryKey = "workout-tracker.history";

    public async Task<List<WorkoutSession>> GetAllAsync()
    {
        var history = await localStorage.GetItemAsync<List<WorkoutSession>>(HistoryKey) ?? [];
        return [.. history.OrderByDescending(s => s.Date)];
    }

    public async Task<WorkoutSession?> GetByIdAsync(Guid id)
    {
        var history = await GetAllAsync();
        return history.FirstOrDefault(s => s.Id == id);
    }

    public async Task AddAsync(WorkoutSession session)
    {
        var history = await localStorage.GetItemAsync<List<WorkoutSession>>(HistoryKey) ?? [];
        history.Add(session);
        await localStorage.SetItemAsync(HistoryKey, history);
    }

    public async Task<WorkoutExerciseEntry?> GetLastExerciseRecordAsync(string exerciseName)
    {
        var history = await GetAllAsync();
        foreach (var session in history)
        {
            var entry = session.Exercises.FirstOrDefault(e => e.ExerciseName == exerciseName);
            if (entry is not null)
            {
                return entry;
            }
        }

        return null;
    }
}
