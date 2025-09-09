namespace MongoDB.Application.DTOs;

public class SampleDto
{
    public string Id { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Hash { get; set; }
    public int LC_FC { get; set; }
    public List<SampleDataDto> Sample { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SampleDataDto
{
    public float Data { get; set; }
    public DateTime VahicleDate { get; set; }
    public DateTime RecievedDate { get; set; }
}