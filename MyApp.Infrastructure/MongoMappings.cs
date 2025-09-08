using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MyApp.Domain;

namespace MyApp.Infrastructure;

internal static class MongoMappings
{
	private static bool _initialized;
	private static readonly object _lock = new();

	public static void EnsureInitialized()
	{
		if (_initialized) return;
		lock (_lock)
		{
			if (_initialized) return;
			Register();
			_initialized = true;
		}
	}

	private static void Register()
	{
		if (!BsonClassMap.IsClassMapRegistered(typeof(SampleItem)))
		{
			BsonClassMap.RegisterClassMap<SampleItem>(cm =>
			{
				cm.AutoMap();
				cm.MapMember(x => x.VehicleDate).SetElementName("Vahicle Date");
				cm.MapMember(x => x.ReceivedDate).SetElementName("recieved Date");
				cm.MapMember(x => x.Name).SetElementName("Name");
			});
		}

		if (!BsonClassMap.IsClassMapRegistered(typeof(MyDocument)))
		{
			BsonClassMap.RegisterClassMap<MyDocument>(cm =>
			{
				cm.AutoMap();
				cm.MapIdMember(x => x.Id)
					.SetIdGenerator(MongoDB.Bson.Serialization.IdGenerators.StringObjectIdGenerator.Instance)
					.SetSerializer(new StringSerializer(BsonType.ObjectId));
				cm.MapMember(x => x.Date).SetElementName("date");
				cm.MapMember(x => x.Name).SetElementName("Name");
				cm.MapMember(x => x.Hash).SetElementName("Hash");
				cm.MapMember(x => x.LcFc).SetElementName("LC_FC");
				cm.MapMember(x => x.Samples).SetElementName("sample");
			});
		}
	}
}

