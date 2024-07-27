namespace WateringSystem.Commands.Status;

public class StatusResponse
{
    public double Distance { get; set; }
    public bool Pump { get; set; }
    public bool Strawberries { get; set; }
    public bool Tomatoes { get; set; }
    public List<int> TomatoesHours  { get; set; }
    public List<int> StrawberriesHours { get; set; }
    public List<int> PumpingHours { get; set; }
    
    public string LastTomatoesWatering { get; set; }
    
    public string LastStrawberriesWatering { get; set; }
    
    public string LastPumpingDate { get; set; }
    
    public int LastTomatoesWateringWaterConsumption { get; set; }
    
    public int LastStrawberriesWateringConsumption { get; set; }
    
    public int LastPumpCycleWaterAdded { get; set; }
    
    public double ExternalTemperature { get; set; }
    
    public double ExternalHumidity { get; set; }
    
    public bool IrrigationOn { get; set; }
    
    public bool IrrigationTomatoes { get; set; }
    
    public bool IrrigationStrawberries { get; set; }
    
    public bool PumpOn { get; set; }
}