using System.Threading;
using System.Threading.Tasks;

namespace Torisho.Domain.Interfaces.Repositories;

public interface IUnitOfWork
{
	IUserRepository Users { get; }
	IDictionaryEntryRepository DictionaryEntries { get; }
	ILevelRepository Levels { get; }
	IQuizRepository Quizzes { get; }
	IQuestionRepository Questions { get; }
	IQuizAttemptRepository QuizAttempts { get; }
	IRoomRepository Rooms { get; }
	IVideoLessonRepository VideoLessons { get; }
	IVideoProgressRepository VideoProgresses { get; }
	ILearningProgressRepository LearningProgress { get; }
	IChapterProgressRepository ChapterProgress { get; }
	IDailyActivitiesRepository DailyActivities { get; }

	Task<int> SaveChangesAsync(CancellationToken ct = default);
}
