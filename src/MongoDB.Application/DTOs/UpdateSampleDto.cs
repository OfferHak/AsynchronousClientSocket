namespace MongoDB.Application.DTOs;

public class UpdateSampleDto
{
    public string Id { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Hash { get; set; }
    public int LC_FC { get; set; }
    public List<UpdateSampleDataDto> Sample { get; set; } = new();
}

public class UpdateSampleDataDto
{
    public float Data { get; set; }
    public DateTime VahicleDate { get; set; }
    public DateTime RecievedDate { get; set; }
}