using DataReader.Types;

namespace RandomForest;

public static class Parser
{
    public static List<Row> GetRows(Data data, DateTime start)
    {
        List<Row> rows = new List<Row>();
        data.Telemetries.ForEach(x =>
        {
            var row = new Row() { Parameters = new List<double>() { x.Pressure, x.Rotate, x.Vibration, x.Volt, (x.DateTime - start).TotalHours }, Output = 0 };
            var lastMaint = data.Maints.LastOrDefault(m => m.DateTime < x.DateTime);
            if (lastMaint == null) lastMaint = new Maint() { DateTime = start };
            var nextFail = data.Failures.FirstOrDefault(f => f.DateTime > x.DateTime);
            if (nextFail != null)
            {
                var countErrors = data.Errors.Count(er => er.DateTime > lastMaint.DateTime && er.DateTime < x.DateTime);
                var timeOfLastMaint = (x.DateTime - lastMaint.DateTime).TotalHours;
                var timeOfNextFailt = (nextFail.DateTime - x.DateTime).TotalHours;
                //row.Parameters.Add(countErrors);
                //row.Parameters.Add(timeOfLastMaint);
                row.Output = timeOfNextFailt;
                rows.Add(row);
            }
        });
        return rows;
    }

    public static Row GetRow(Data data, Telemetry telemetry)
    {
        var first = data.Telemetries.First();
        var row = new Row() { Parameters = new List<double>() { telemetry.Pressure, telemetry.Rotate, telemetry.Vibration, telemetry.Volt, (telemetry.DateTime - first.DateTime).TotalHours }, Output = 0 };
        var lastMaint = data.Maints.LastOrDefault(m => m.DateTime < telemetry.DateTime);
        if (lastMaint == null) lastMaint = new Maint() { DateTime = first.DateTime };
        var nextFail = data.Failures.FirstOrDefault(f => f.DateTime > telemetry.DateTime);
        if (nextFail != null)
        {
            var countErrors =
                data.Errors.Count(er => er.DateTime > lastMaint.DateTime && er.DateTime < telemetry.DateTime);
            var timeOfLastMaint = (telemetry.DateTime - lastMaint.DateTime).TotalHours;
            var timeOfNextFailt = (nextFail.DateTime - telemetry.DateTime).TotalHours;
            //row.Parameters.Add(countErrors);
            row.Parameters.Add(timeOfLastMaint);
            row.Output = timeOfNextFailt;
            return row;
        }
        else return null;
    }
}