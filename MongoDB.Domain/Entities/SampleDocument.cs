using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB.Domain.Entities;

public class SampleDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("Name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("Hash")]
    public double Hash { get; set; }

    [BsonElement("LC_FC")]
    public int LC_FC { get; set; }

    [BsonElement("sample")]
    public List<SampleData> Sample { get; set; } = new();
}

public class SampleData
{
    [BsonElement("Data")]
    public float Data { get; set; }

    [BsonElement("Vahicle Date")]
    public DateTime VehicleDate { get; set; }

    [BsonElement("recieved Date")]
    public DateTime ReceivedDate { get; set; }
}