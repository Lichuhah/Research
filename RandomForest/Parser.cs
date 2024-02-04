using DataReader.Types;

namespace RandomForest;

public static class Parser
{
    public static List<Row> GetRowsForType(List<Data> data, DateTime start)
    {
        List<Row> rows = new List<Row>();
        data.ForEach(d =>
        {
            foreach (var keyValuePair in d.Parameters)
            {
                rows.Add(new Row()
                {
                    Parameters = keyValuePair.Value,
                    Output = d.Output[keyValuePair.Key]
                });
            }
        });
        return rows;
    }
    
    
    public static List<Row> GetRows(Data data, DateTime start)
    {
        List<Row> rows = new List<Row>();
        data.Telemetries.ForEach(x =>
        {
            var row = new Row() { Parameters = new List<double>() { x.Pressure, x.Rotate, x.Vibration, x.Volt}, Output = 0 };
            var nextFail = data.Failures.FirstOrDefault(f => f.DateTime > x.DateTime);
            if (nextFail != null )
            {
                var lastMaint = data.Maints.LastOrDefault(m => m.DateTime < x.DateTime);
                if (lastMaint == null) lastMaint = new Maint() { DateTime = start };
                var countErrors = data.Errors.Count(er => er.DateTime > lastMaint.DateTime && er.DateTime < x.DateTime);
                var timeOfNextFault = (nextFail.DateTime - x.DateTime).TotalDays;
                row.Parameters.Add(countErrors);
                if (timeOfNextFault != 0)
                {
                    var timeOfLastMaint = (x.DateTime - lastMaint.DateTime).TotalDays;
                    row.Parameters.Add(timeOfLastMaint);
                    row.Output = timeOfNextFault;
                    rows.Add(row);
                }
            }
        });
        return rows;
    }

    public static Row? GetRow(Data data, Telemetry telemetry)
    {
        DateTime start = data.Telemetries.First().DateTime;
        var nextFail = data.Failures.FirstOrDefault(f => f.DateTime > telemetry.DateTime);
        if (nextFail != null)
        {
            var lastMaint = data.Maints.LastOrDefault(m => m.DateTime < telemetry.DateTime);
            if (lastMaint == null) lastMaint = new Maint() { DateTime = start };
            var timeOfNextFault = (int)(nextFail.DateTime - telemetry.DateTime).TotalDays;
            if (timeOfNextFault != 0)
            {
                /*data.Errors.Select(x => x.ErrorId).ToList().ForEach(erType =>
                {
                    row.Parameters.Add(data.Errors.Where(x => x.ErrorId == erType)
                        .Count(er =>
                            er.DateTime > lastMaint.DateTime && er.DateTime < telemetry.DateTime)); //кол-во ошибок по типам от последнего обслуживания
                });*/
                var row = new Row() { Parameters = new List<double>()
                {
                    telemetry.Pressure / (data.Telemetries.FirstOrDefault(x=>x.DateTime >= x.DateTime.AddDays(-7))?.Pressure ?? 1),
                    telemetry.Pressure / (data.Telemetries.FirstOrDefault(x=>x.DateTime >= x.DateTime.AddDays(-3))?.Pressure ?? 1),
                    telemetry.Pressure / (data.Telemetries.FirstOrDefault(x=>x.DateTime >= x.DateTime.AddDays(-1))?.Pressure ?? 1),
                    telemetry.Pressure / data.Telemetries.First(x=>x.DateTime >= lastMaint.DateTime).Pressure,
                    telemetry.Rotate / (data.Telemetries.FirstOrDefault(x=>x.DateTime >= x.DateTime.AddDays(-7))?.Rotate ?? 1),
                    telemetry.Rotate / (data.Telemetries.FirstOrDefault(x=>x.DateTime >= x.DateTime.AddDays(-3))?.Rotate ?? 1),
                    telemetry.Rotate / (data.Telemetries.FirstOrDefault(x=>x.DateTime >= x.DateTime.AddDays(-1))?.Rotate ?? 1),
                    telemetry.Rotate / data.Telemetries.First(x=>x.DateTime >= lastMaint.DateTime).Rotate,
                    telemetry.Vibration / (data.Telemetries.FirstOrDefault(x=>x.DateTime >= x.DateTime.AddDays(-7))?.Vibration ?? 1),
                    telemetry.Vibration / (data.Telemetries.FirstOrDefault(x=>x.DateTime >= x.DateTime.AddDays(-3))?.Vibration ?? 1),
                    telemetry.Vibration / (data.Telemetries.FirstOrDefault(x=>x.DateTime >= x.DateTime.AddDays(-1))?.Vibration ?? 1),
                    telemetry.Vibration / data.Telemetries.First(x=>x.DateTime >= lastMaint.DateTime).Vibration,
                    telemetry.Volt / (data.Telemetries.FirstOrDefault(x=>x.DateTime >= x.DateTime.AddDays(-7))?.Volt ?? 1),
                    telemetry.Volt / (data.Telemetries.FirstOrDefault(x=>x.DateTime >= x.DateTime.AddDays(-3))?.Volt ?? 1),
                    telemetry.Volt / (data.Telemetries.FirstOrDefault(x=>x.DateTime >= x.DateTime.AddDays(-1))?.Volt ?? 1),
                    telemetry.Volt / data.Telemetries.First(x=>x.DateTime >= lastMaint.DateTime).Volt,
                    data.Age
                }, Output = 0 }; 
                var timeOfLastMaint = (int)(telemetry.DateTime - lastMaint.DateTime).TotalDays;
                row.Parameters.Add(data.Errors.Count(x=>x.DateTime>lastMaint.DateTime && x.DateTime <= telemetry.DateTime && x.ErrorId==1)); //время последнего обслуживания
                row.Parameters.Add(data.Errors.Count(x=>x.DateTime>lastMaint.DateTime && x.DateTime <= telemetry.DateTime && x.ErrorId==2)); //время последнего обслуживания
                row.Parameters.Add(data.Errors.Count(x=>x.DateTime>lastMaint.DateTime && x.DateTime <= telemetry.DateTime && x.ErrorId==3)); //время последнего обслуживания
                row.Parameters.Add(data.Errors.Count(x=>x.DateTime>lastMaint.DateTime && x.DateTime <= telemetry.DateTime && x.ErrorId==4)); //время последнего обслуживания
                row.Parameters.Add(data.Maints.Count(x=>x.DateTime > start));
                row.Parameters.Add(data.Failures.Count(x=>x.DateTime > start));
                row.Parameters.Add(timeOfLastMaint); //время последнего обслуживания
                row.Output = timeOfNextFault; //след сбой
                return row;
            }
            else return null;
        }
        else return null;
    }
}