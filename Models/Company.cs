using project_basic.Interfaces;

namespace project_basic.Models;

public class Company: BaseEntity, ISoftDelete
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } =  string.Empty;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public Guid UserId { get; set; }   // FK
    public User User { get; set; } = null!;
}