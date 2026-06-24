namespace TownHubPeople.Domain.Common;

public class DocumentSequence
{
    public int Id { get; set; }
    public string Prefix { get; set; } = string.Empty;
    public int LastNumber { get; set; }
    public string Format { get; set; } = "{0}-{1:D6}";
}
