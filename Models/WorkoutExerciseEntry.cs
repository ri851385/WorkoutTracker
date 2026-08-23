namespace WorkoutTracker.Models;

public class WorkoutExerciseEntry
{
    public required string ExerciseName { get; set; }
    public List<SetEntry> Sets { get; set; } = [];
}
