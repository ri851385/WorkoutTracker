namespace WorkoutTracker.Models;

public class WorkoutExerciseEntry
{
    public required string ExerciseName { get; set; }
    public string? Note { get; set; }
    public List<SetEntry> Sets { get; set; } = [];
}
