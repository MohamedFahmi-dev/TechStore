using Microsoft.AspNetCore.Identity;

namespace TechStore.Domain.Common;

public abstract class AuditableIdentityUser : IdentityUser<int>, IAuditableEntity, ISoftDeleteEntity
{
    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedOnUtc { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOnUtc { get; set; }

    public void Delete()
    {
        IsDeleted = true;
        DeletedOnUtc = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedOnUtc = null;
    }
}
