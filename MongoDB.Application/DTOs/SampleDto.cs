namespace MongoDB.Application.DTOs;

public class SampleDto
{
    public string Id { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Hash { get; set; }
    public int LC_FC { get; set; }
    public List<SampleDataDto> Sample { get; set; } = new();
}

public class SampleDataDto
{
    public float Data { get; set; }
    public DateTime VehicleDate { get; set; }
    public DateTime ReceivedDate { get; set; }
}

public class CreateSampleDto
{
    public DateTime Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Hash { get; set; }
    public int LC_FC { get; set; }
    public List<SampleDataDto> Sample { get; set; } = new();
}

public class UpdateSampleDto
{
    public DateTime Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Hash { get; set; }
    public int LC_FC { get; set; }
    public List<SampleDataDto> Sample { get; set; } = new();
}

public class PagedResultDto<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public class ClientRegistrationDto
{
    public string ClientName { get; set; } = string.Empty;
    public string ConnectionId { get; set; } = string.Empty;
}