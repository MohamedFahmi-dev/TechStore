namespace TechStore.Domain.Common;

public interface IAuditableEntity
{
    DateTime CreatedOnUtc { get; set; }
    DateTime? ModifiedOnUtc { get; set; }
}
