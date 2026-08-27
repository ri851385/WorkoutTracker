namespace WorkoutTracker.Models;

public class WorkoutSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; } = DateTime.Now;
    public DateTime? CompletedDate { get; set; }
    public decimal? BodyWeightKg { get; set; }
    public string Name { get; set; } = "";
    public bool IsCompleted { get; set; }
    public List<WorkoutExerciseEntry> Exercises { get; set; } = [];
}
