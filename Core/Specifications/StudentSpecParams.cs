namespace Core.Specifications;

public class StudentSpecParams : PagingParams
{
    private string? _search;

    public string Search
    {
        get => _search ?? "";
        set => _search = value.ToLower();
    }

    public string? Sort { get; set; }
}
