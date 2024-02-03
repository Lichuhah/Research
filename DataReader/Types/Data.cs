namespace DataReader.Types;

public class Data
{
    public int Id { get; set; }
    public int Model { get; set; }
    public int Age { get; set; }
    public List<Error> Errors { get; set; }
    public List<Failure> Failures { get; set; }
    public List<Maint> Maints { get; set; }
    public List<Telemetry> Telemetries { get; set; }

    public void FilterByTime(DateTime time)
    {
        this.Errors = this.Errors.Where(x => x.DateTime < time).ToList();
        this.Failures = this.Failures.Where(x => x.DateTime < time).ToList();
        this.Maints = this.Maints.Where(x => x.DateTime < time).ToList();
        this.Telemetries = this.Telemetries.Where(x => x.DateTime < time).ToList();
    }
}