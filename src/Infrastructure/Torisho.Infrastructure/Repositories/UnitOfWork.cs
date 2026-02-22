using Torisho.Application;
using Torisho.Domain.Interfaces.Repositories;

namespace Torisho.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDataContext _context;

    // Lazy initialization của repositories
    private IUserRepository? _users;
    private IDictionaryEntryRepository? _dictionaryEntries;
    private ILevelRepository? _levels;
    private IQuizRepository? _quizzes;
    private IQuestionRepository? _questions;
    private IQuizAttemptRepository? _quizAttempts;
    private IRoomRepository? _rooms;
    private IVideoLessonRepository? _videoLessons;
    private IVideoProgressRepository? _videoProgresses;
    private ILearningProgressRepository? _learningProgress;
    private IChapterProgressRepository? _chapterProgress;
    private IDailyActivitiesRepository? _dailyActivities;

    public UnitOfWork(IDataContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // Properties với lazy initialization
    public IUserRepository Users => 
        _users ??= new UserRepository(_context);

    public IDictionaryEntryRepository DictionaryEntries => 
        _dictionaryEntries ??= new DictionaryEntryRepository(_context);

    public ILevelRepository Levels => 
        _levels ??= new LevelRepository(_context);

    public IQuizRepository Quizzes => 
        _quizzes ??= new QuizRepository(_context);

    public IQuestionRepository Questions => 
        _questions ??= new QuestionRepository(_context);

    public IQuizAttemptRepository QuizAttempts => 
        _quizAttempts ??= new QuizAttemptRepository(_context);

    public IRoomRepository Rooms => 
        _rooms ??= new RoomRepository(_context);

    public IVideoLessonRepository VideoLessons => 
        _videoLessons ??= new VideoLessonRepository(_context);

    public IVideoProgressRepository VideoProgresses => 
        _videoProgresses ??= new VideoProgressRepository(_context);

    public ILearningProgressRepository LearningProgress => 
        _learningProgress ??= new LearningProgressRepository(_context);

    public IChapterProgressRepository ChapterProgress => 
        _chapterProgress ??= new ChapterProgressRepository(_context);

    public IDailyActivitiesRepository DailyActivities => 
        _dailyActivities ??= new DailyActivitiesRepository(_context);

    // Transaction commit point
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }
}