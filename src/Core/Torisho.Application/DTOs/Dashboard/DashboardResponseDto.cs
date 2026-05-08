namespace Torisho.Application.DTOs.Dashboard;

public sealed record DashboardResponseDto
{
    public DashboardProfileDto Profile { get; init; } = new();
    public DashboardTodayDto Today { get; init; } = new();
    public List<DashboardLevelProgressDto> ProgressByLevel { get; init; } = new();
    public DashboardStreakDto Streak { get; init; } = new();
    public DashboardContinueLearningDto? ContinueLearning { get; init; }
    public DashboardQuickStatsDto QuickStats { get; init; } = new();
    public DashboardCalendarDto Calendar { get; init; } = new();
}

public sealed record DashboardProfileDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
}

public sealed record DashboardTodayDto
{
    public DateOnly Date { get; init; }
    public string Timezone { get; init; } = "Asia/Saigon";
    public DashboardDailyWordDto? DailyWord { get; init; }
}

public sealed record DashboardDailyWordDto
{
    public Guid EntryId { get; init; }
    public string Term { get; init; } = string.Empty;
    public string Reading { get; init; } = string.Empty;
    public string Meaning { get; init; } = string.Empty;
}

public sealed record DashboardLevelProgressDto
{
    public Guid LevelId { get; init; }
    public string LevelCode { get; init; } = string.Empty;
    public string LevelName { get; init; } = string.Empty;
    public float CompletionPercent { get; init; }
    public float VocabularyProgress { get; init; }
    public float GrammarProgress { get; init; }
    public float ReadingProgress { get; init; }
    public int CompletedChapters { get; init; }
    public int TotalChapters { get; init; }
    public string Status { get; init; } = string.Empty;
}

public sealed record DashboardStreakDto
{
    public int Current { get; init; }
    public int Longest { get; init; }
    public bool StudiedToday { get; init; }
}

public sealed record DashboardContinueLearningDto
{
    public Guid LevelId { get; init; }
    public string LevelCode { get; init; } = string.Empty;
    public Guid ChapterId { get; init; }
    public string ChapterTitle { get; init; } = string.Empty;
    public Guid LessonId { get; init; }
    public string LessonSlug { get; init; } = string.Empty;
    public string LessonTitle { get; init; } = string.Empty;
    public float LessonProgressPercent { get; init; }
    public string? CurrentSection { get; init; }
    public DateTime LastUpdated { get; init; }
}

public sealed record DashboardQuickStatsDto
{
    public int VocabularyLearned { get; init; }
    public int GrammarLearned { get; init; }
    public int KanjiLearned { get; init; }
}

public sealed record DashboardCalendarDto
{
    public int Year { get; init; }
    public int Month { get; init; }
    public List<DateOnly> StudyDates { get; init; } = new();
}
