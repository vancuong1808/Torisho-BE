using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.UserDomain;

public sealed class Permission : BaseEntity
{
   public string Code { get; private set; } = default!;
   public string Description { get; private set; } = default!;

   private Permission() { }

   public Permission(string code, string description)
   {
      Code = code;
      Description = description;
   }

   public bool CheckAccess() => true;
}