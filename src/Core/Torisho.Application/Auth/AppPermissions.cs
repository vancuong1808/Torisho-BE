namespace Torisho.Application.Auth;

public static class AppPermissions
{
    public const string AdminAccess = "admin.access";
    public const string UsersRead = "users.read";
    public const string UsersManage = "users.manage";
    public const string ContentRead = "content.read";
    public const string ContentManage = "content.manage";
    public const string CurriculumImport = "curriculum.import";
    public const string QuizManage = "quiz.manage";
    public const string DictionaryManage = "dictionary.manage";
    public const string CommentsModerate = "comments.moderate";
    public const string RoomsMonitor = "rooms.monitor";

    public static readonly IReadOnlyCollection<string> AdminPermissions =
    [
        AdminAccess,
        UsersRead,
        UsersManage,
        ContentRead,
        ContentManage,
        CurriculumImport,
        QuizManage,
        DictionaryManage,
        CommentsModerate,
        RoomsMonitor
    ];

    public static readonly IReadOnlyCollection<string> UserPermissions =
    [
        ContentRead
    ];
}
