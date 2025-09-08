namespace MyApp.Domain;

public sealed class SampleItem
{
	public DateTime? VehicleDate { get; set; }
	public DateTime? ReceivedDate { get; set; }
	public string? Name { get; set; }
}

public sealed class MyDocument
{
	public string? Id { get; set; }
	public DateTime? Date { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Hash { get; set; }
	public string? LcFc { get; set; }
	public List<SampleItem> Samples { get; set; } = new();
}

public interface IAuditableEntity
{
	DateTime CreatedAtUtc { get; }
	DateTime? UpdatedAtUtc { get; }
}
