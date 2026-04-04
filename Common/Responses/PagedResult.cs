namespace project_basic.Common.Responses;

public class PagedResult<T>
{
    public Meta Meta { get; set; }
    public List<T> Result { get; set; }
}