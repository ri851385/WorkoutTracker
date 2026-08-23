namespace WorkoutTracker.Models;

public class ExerciseCategory
{
    public required string Name { get; set; }
    public required List<string> Exercises { get; set; }
}
