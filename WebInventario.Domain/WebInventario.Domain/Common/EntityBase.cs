namespace WebInventario.Domain.Common
{
    public abstract class EntityBase
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();

        // Auditoría
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Concurrencia optimista
        public byte[] RowVersion { get; protected set; } = Array.Empty<byte>();
    }
}
