using mood_tracker.Models;

namespace mood_tracker.Services;

public interface IEntryStore
{
    IReadOnlyList<MoodEntry> GetAll();
    MoodEntry? GetByDate(DateOnly date);
    void Upsert(MoodEntry entry);
}
