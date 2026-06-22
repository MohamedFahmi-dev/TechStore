namespace TechStore.Domain.Common;

public abstract class BaseEntity : IAuditableEntity, ISoftDeleteEntity
{
    public int Id { get; set; }
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
