using WateringSystem.Data.Entities;

namespace WateringSystem.Data;

public interface IStatusRepository
{
    Task InsertStatusAsync(Status status);
    Task<Status> GetLastStatusAsync();
}