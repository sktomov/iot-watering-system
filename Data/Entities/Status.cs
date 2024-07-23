namespace WateringSystem.Data.Entities;

public class Status
{
    public double Distance { get; set; }

    public bool Pump { get; set; }
    
    public bool Strawberries { get; set; }
    
    public bool Tomatoes { get; set; }
    
    public List<int> TomatoesHours { get; set; }

    public List<int> StrawberriesHours { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.Now;
}