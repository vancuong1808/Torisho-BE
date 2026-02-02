using System.ComponentModel.DataAnnotations;

namespace Torisho.Domain.Common;

public class BaseEntity
{
    [Key]
    public Guid Id {get; protected set;} = Guid.NewGuid();
    public DateTime CreatedAt {get; protected set;} = DateTime.UtcNow;
    public DateTime UpdatedAt {get; protected set;} = DateTime.UtcNow;
}