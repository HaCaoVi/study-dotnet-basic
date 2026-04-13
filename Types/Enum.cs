using System.ComponentModel;

namespace project_basic.Types;

public enum EGenderType
{
    [Description("Unknown")]
    Unknown = 0,
    [Description("Male")]
    Male = 1,
    [Description("Female")]
    Female = 2,
    [Description("Other")]
    Other = 3
}

public enum EMethod
{
    [Description("GET")]
    Get = 0,
    [Description("POST")]
    Post = 1,
    [Description("PUT")]
    Put = 2,
    [Description("DELETE")]
    Delete = 3
}
