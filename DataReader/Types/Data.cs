namespace DataReader.Types;

public class Data
{
    public int Id { get; set; }
    public int Model { get; set; }
    public int Age { get; set; }
    public List<Error> Errors { get; set; }
    public List<Failure> Failures { get; set; }
    public List<Maint> Maints { get; set; }
    public List<Telemetry> _Telemetries { get; set; }
    public List<Telemetry> Telemetries { get; set; }
    public Dictionary<int, List<double>> Parameters { get; set; }
    public Dictionary<int, double> Output { get; set; }
    
    public void FilterByTime(DateTime time)
    {
        this.Errors = this.Errors.Where(x => x.DateTime < time).ToList();
        this.Failures = this.Failures.Where(x => x.DateTime < time).ToList();
        this.Maints = this.Maints.Where(x => x.DateTime < time).ToList();
        this.Telemetries = this.Telemetries.Where(x => x.DateTime < time).ToList();
    }

    public void FilterBeforeLastFailure(DateTime end)
    {
        this.Telemetries = new List<Telemetry>(_Telemetries.Where(x => x.DateTime > _Telemetries[0].DateTime.AddDays(30)).ToList());
        DateTime firstMaint = this.Maints.First().DateTime;
        DateTime lastFail = this.Failures.Where(x => x.DateTime < end).Last().DateTime;
        Telemetries = Telemetries.Where(x => x.DateTime < lastFail && x.DateTime > firstMaint).ToList();
    }

    public void InitParameters()
    {
        Parameters = new Dictionary<int, List<double>>();
        Output = new Dictionary<int, double>();
        List<Telemetry> tels = Telemetries.Where(x => x.DateTime > Telemetries[0].DateTime.AddDays(30)).ToList();
        for (var i = 0; i < this.Telemetries.Count; i++)
        {
            GetRow(i);
        }
    }

    public void GetRow(int telemetryId)
    {
        Telemetry telemetry = this.Telemetries[telemetryId];
        DateTime start = this.Telemetries.First().DateTime;
        List<double> Parameters = new List<double>();
        var nextFail = this.Failures.First(f => f.DateTime > telemetry.DateTime);
        var lastMaint = this.Maints.Last(m => m.DateTime < telemetry.DateTime);
        var timeOfNextFault = (int)(nextFail.DateTime - telemetry.DateTime).TotalDays;
        Parameters = new List<double>()
            {
                telemetry.Pressure,
                telemetry.Rotate,
                telemetry.Vibration,
                telemetry.Volt,
                telemetry.Pressure / (this._Telemetries.FirstOrDefault(x => x.DateTime >= telemetry.DateTime.AddDays(-7))
                    ?.Pressure ?? 1),
                telemetry.Pressure / (this._Telemetries.FirstOrDefault(x => x.DateTime >= telemetry.DateTime.AddDays(-3))
                    ?.Pressure ?? 1),
                telemetry.Pressure / (this._Telemetries.FirstOrDefault(x => x.DateTime >= telemetry.DateTime.AddDays(-1))
                    ?.Pressure ?? 1),
                telemetry.Pressure / this._Telemetries.First(x => x.DateTime >= lastMaint.DateTime).Pressure,
                telemetry.Rotate / (this._Telemetries.FirstOrDefault(x => x.DateTime >= telemetry.DateTime.AddDays(-7))
                    ?.Rotate ?? 1),
                telemetry.Rotate / (this._Telemetries.FirstOrDefault(x => x.DateTime >= telemetry.DateTime.AddDays(-3))
                    ?.Rotate ?? 1),
                telemetry.Rotate / (this._Telemetries.FirstOrDefault(x => x.DateTime >= telemetry.DateTime.AddDays(-1))
                    ?.Rotate ?? 1),
                telemetry.Rotate / this._Telemetries.First(x => x.DateTime >= lastMaint.DateTime).Rotate,
                telemetry.Vibration / (this._Telemetries
                    .FirstOrDefault(x => x.DateTime >= telemetry.DateTime.AddDays(-7))?.Vibration ?? 1),
                telemetry.Vibration / (this._Telemetries
                    .FirstOrDefault(x => x.DateTime >= telemetry.DateTime.AddDays(-3))?.Vibration ?? 1),
                telemetry.Vibration / (this._Telemetries
                    .FirstOrDefault(x => x.DateTime >= telemetry.DateTime.AddDays(-1))?.Vibration ?? 1),
                telemetry.Vibration / this._Telemetries.First(x => x.DateTime >= lastMaint.DateTime).Vibration,
                telemetry.Volt / (this._Telemetries.FirstOrDefault(x => x.DateTime >= telemetry.DateTime.AddDays(-7))
                    ?.Volt ?? 1),
                telemetry.Volt / (this._Telemetries.FirstOrDefault(x => x.DateTime >= telemetry.DateTime.AddDays(-3))
                    ?.Volt ?? 1),
                telemetry.Volt / (this._Telemetries.FirstOrDefault(x => x.DateTime >= telemetry.DateTime.AddDays(-1))
                    ?.Volt ?? 1),
                telemetry.Volt / this._Telemetries.First(x => x.DateTime >= lastMaint.DateTime).Volt,
                this.Age
            };
            var timeOfLastMaint = (int)(telemetry.DateTime - lastMaint.DateTime).TotalDays;
            /*Parameters.Add(this.Errors.Count(x =>
                x.DateTime > lastMaint.DateTime && x.DateTime <= telemetry.DateTime &&
                x.ErrorId == 1)); //время последнего обслуживания
            Parameters.Add(this.Errors.Count(x =>
                x.DateTime > lastMaint.DateTime && x.DateTime <= telemetry.DateTime &&
                x.ErrorId == 2)); //время последнего обслуживания
            Parameters.Add(this.Errors.Count(x =>
                x.DateTime > lastMaint.DateTime && x.DateTime <= telemetry.DateTime &&
                x.ErrorId == 3)); //время последнего обслуживания
            Parameters.Add(this.Errors.Count(x =>
                x.DateTime > lastMaint.DateTime && x.DateTime <= telemetry.DateTime &&
                x.ErrorId == 4)); //время последнего обслуживания*/
            //Parameters.Add(this.Maints.Count(x => x.DateTime > start));
            //Parameters.Add(this.Failures.Count(x => x.DateTime > start));
            Parameters.Add(timeOfLastMaint); //время последнего обслуживания
            Output.Add(telemetryId, timeOfNextFault); //след сбой
            this.Parameters.Add(telemetryId, Parameters);
    }
}