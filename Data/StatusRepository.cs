using MongoDB.Driver;
using WateringSystem.Data.Entities;

namespace WateringSystem.Data;

public class StatusRepository : IStatusRepository
{
    private readonly IMongoCollection<Status> _statuses;
    
    public StatusRepository(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
        var database = client.GetDatabase("polyantsi");
        _statuses = database.GetCollection<Status>("statuses");
    }

    public async Task InsertStatusAsync(Status status)
        => await _statuses.InsertOneAsync(status);
}