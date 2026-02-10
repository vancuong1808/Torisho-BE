using Torisho.Domain.Common;

namespace Torisho.Domain.Entities.UserDomain;

public sealed class Permission : BaseEntity
{
   public string Code { get; private set; } = string.Empty;
   public string Description { get; private set; } = string.Empty;

   private Permission() { }

   public Permission(string code, string description)
   {
      if (string.IsNullOrWhiteSpace(code))
         throw new ArgumentException("Code is required", nameof(code));
      if (string.IsNullOrWhiteSpace(description))
         throw new ArgumentException("Description is required", nameof(description));

      Code = code;
      Description = description;
   }

   public bool CheckAccess() => true;
}