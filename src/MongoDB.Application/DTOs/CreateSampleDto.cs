namespace MongoDB.Application.DTOs;

public class CreateSampleDto
{
    public DateTime Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Hash { get; set; }
    public int LC_FC { get; set; }
    public List<CreateSampleDataDto> Sample { get; set; } = new();
}

public class CreateSampleDataDto
{
    public float Data { get; set; }
    public DateTime VahicleDate { get; set; }
    public DateTime RecievedDate { get; set; }
}