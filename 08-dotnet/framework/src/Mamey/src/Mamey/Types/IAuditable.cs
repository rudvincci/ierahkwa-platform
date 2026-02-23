namespace Mamey.Types;

    public interface IAuditable
    {
        Guid CreatedBy { get; }
        Guid? ModifiedBy { get; }
        DateTime CreatedAt { get; }
        DateTime? ModifiedAt { get; }
    }

    public interface IAuditable<T> 
    {
        public new T CreatedBy { get; }
        public new T? ModifiedBy { get; }
        public DateTime CreatedAt { get; }
        public DateTime? ModifiedAt { get; }
        public void Touch(T modifier);

        public void SetAudit(T createdBy);
    }