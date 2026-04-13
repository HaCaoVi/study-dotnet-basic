using project_basic.Types;

namespace project_basic.Entities;

public class Permission: BaseEntity
{
    public Guid  Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public EMethod Method { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<RolePermission> RolePermissions { get; set; } = new();
}