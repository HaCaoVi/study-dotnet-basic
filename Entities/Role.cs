namespace project_basic.Models;

public class Role: BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<RolePermission> RolePermissions { get; set; } = new();
    public List<User> Users { get; set; } = new();
}