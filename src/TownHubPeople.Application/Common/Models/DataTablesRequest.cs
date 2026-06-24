namespace TownHubPeople.Application.Common.Models;

public class DataTablesRequest
{
    public int Draw { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }
    public DataTablesSearch? Search { get; set; }
    public List<DataTablesColumn> Columns { get; set; } = [];
    public List<DataTablesOrder> Order { get; set; } = [];
}

public class DataTablesSearch
{
    public string Value { get; set; } = string.Empty;
    public bool Regex { get; set; }
}

public class DataTablesColumn
{
    public string Data { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool Searchable { get; set; }
    public bool Orderable { get; set; }
    public DataTablesSearch? Search { get; set; }
}

public class DataTablesOrder
{
    public int Column { get; set; }
    public string Dir { get; set; } = "asc";
}
