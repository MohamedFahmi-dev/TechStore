namespace TechStore.Domain.Common;

public interface ISoftDeleteEntity
{
    bool IsDeleted { get; set; }
    DateTime? DeletedOnUtc { get; set; }

    void Delete();
    void Restore();
}
