namespace Torisho.Application.Auth;

public static class AuthorizationPolicies
{
    public const string AdminOnly = "AdminOnly";
    public const string CanImportCurriculum = "CanImportCurriculum";
    public const string CanManageQuiz = "CanManageQuiz";
    public const string CanModerateComments = "CanModerateComments";
}
