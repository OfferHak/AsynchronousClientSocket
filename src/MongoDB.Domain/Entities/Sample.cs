using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB.Domain.Entities;

public class Sample : BaseEntity
{
    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("hash")]
    public double Hash { get; set; }

    [BsonElement("lc_fc")]
    public int LC_FC { get; set; }

    [BsonElement("sample")]
    public List<SampleData> SampleData { get; set; } = new();
}

public class SampleData
{
    [BsonElement("data")]
    public float Data { get; set; }

    [BsonElement("vahicle_date")]
    public DateTime VahicleDate { get; set; }

    [BsonElement("recieved_date")]
    public DateTime RecievedDate { get; set; }
}