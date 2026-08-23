using System.Globalization;
using System.Text;
using WorkoutTracker.Models;

namespace WorkoutTracker.Services;

public class CsvExportService
{
    public string BuildHistoryCsv(List<WorkoutSession> sessions)
    {
        var sb = new StringBuilder();
        sb.AppendLine("日付,トレーニング名,種目名,セット番号,重量(kg),回数");

        foreach (var session in sessions.OrderBy(s => s.Date))
        {
            foreach (var exercise in session.Exercises)
            {
                for (var i = 0; i < exercise.Sets.Count; i++)
                {
                    var set = exercise.Sets[i];
                    sb.AppendLine(string.Join(",",
                        EscapeCsvField(session.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)),
                        EscapeCsvField(session.Name),
                        EscapeCsvField(exercise.ExerciseName),
                        (i + 1).ToString(CultureInfo.InvariantCulture),
                        set.Weight.ToString(CultureInfo.InvariantCulture),
                        set.Reps.ToString(CultureInfo.InvariantCulture)));
                }
            }
        }

        return sb.ToString();
    }

    private static string EscapeCsvField(string field)
    {
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        return field;
    }
}
