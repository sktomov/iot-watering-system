using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WateringSystem.Data.Entities;

public class Status
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }
    public double Distance { get; set; }

    public bool Pump { get; set; }
    
    public bool Strawberries { get; set; }
    
    public bool Tomatoes { get; set; }
    
    public List<int> TomatoesHours { get; set; }

    public List<int> StrawberriesHours { get; set; }
    
    public List<int> PumpingHours { get; set; }
    
    public bool IrrigationOn { get; set; }
    
    public bool IrrigationTomatoes { get; set; }
    
    public bool IrrigationStrawberries { get; set; }
    
    public bool PumpOn { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now.ToLocalTime();
}