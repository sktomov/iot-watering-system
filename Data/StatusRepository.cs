using MongoDB.Driver;
using MongoDB.Driver.Linq;
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

    public async Task<Status> GetLastStatusAsync()
        => await _statuses.Find(_ => true).SortByDescending(x => x.CreatedOn)
            .Limit(1)
            .FirstOrDefaultAsync();

    public async Task<Status> GetLastPumpOnEventAsync()
    => await _statuses.AsQueryable()
            .OrderByDescending(x => x.CreatedOn)
            .FirstOrDefaultAsync(x => x.Pump);
}