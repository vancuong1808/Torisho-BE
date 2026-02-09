namespace Torisho.Domain.Interfaces;

public interface IProgressable
{
    float CalculateProgress(Guid userId);
    void UpdateProgress(Guid userId, float progress);
}
