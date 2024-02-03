using DataReader.Types;

namespace RandomForest;

public static class Parser
{
    public static List<Row> GetRowsForType(List<Data> data, DateTime start)
    {
        List<Row> rows = new List<Row>();
        data.ForEach(d=>d.Telemetries.ForEach(x =>
        {
            var row = new Row() { Parameters = new List<double>() { x.Pressure, x.Rotate, x.Vibration, x.Volt, d.Age}, Output = 0 };
            var nextFail = d.Failures.FirstOrDefault(f => f.DateTime > x.DateTime);
            if (nextFail != null )
            {
                var lastMaint = d.Maints.LastOrDefault(m => m.DateTime < x.DateTime);
                if (lastMaint == null) lastMaint = new Maint() { DateTime = start };
                var timeOfNextFault =  (int)(nextFail.DateTime - x.DateTime).TotalDays;
                if (timeOfNextFault != 0)
                {
                    var countErrors = d.Errors.Count(er => er.DateTime > lastMaint.DateTime && er.DateTime < x.DateTime);
                    var timeOfLastMaint = (int)(x.DateTime - lastMaint.DateTime).TotalDays;
                    row.Parameters.Add(countErrors);
                    row.Parameters.Add(timeOfLastMaint);
                    row.Output = timeOfNextFault;
                    rows.Add(row);
                }
            }
        }));
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

    public static Row GetRow(Data data, Telemetry telemetry)
    {
        var first = data.Telemetries.First();
        var row = new Row() { Parameters = new List<double>() { telemetry.Pressure, telemetry.Rotate, telemetry.Vibration, telemetry.Volt, data.Age}, Output = 0 };
        var nextFail = data.Failures.FirstOrDefault(f => f.DateTime > telemetry.DateTime);
        if (nextFail != null)
        {
            var lastMaint = data.Maints.LastOrDefault(m => m.DateTime < telemetry.DateTime);
            if (lastMaint == null) lastMaint = new Maint() { DateTime = first.DateTime };
            var countErrors =
                data.Errors.Count(er => er.DateTime > lastMaint.DateTime && er.DateTime < telemetry.DateTime);
            var timeOfNextFault =  (int)(nextFail.DateTime - telemetry.DateTime).TotalDays;
            if (timeOfNextFault != 0)
            {
                var timeOfLastMaint =  (int)(telemetry.DateTime - lastMaint.DateTime).TotalDays;
                row.Parameters.Add(countErrors);
                row.Parameters.Add(timeOfLastMaint);
                row.Output = timeOfNextFault;
                return row;
            }
            else return null;
        }
        else return null;
    }
}