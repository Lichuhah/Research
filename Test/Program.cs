using DataReader.Types;
using RandomForest;
using Test;

SimpleTest.Start();
var datas = DataReader.Reader.GetData();
var testdata = datas.Where(x=>x.Model == 1).ToList();

DateTime start = new DateTime(2015, 1, 1);
DateTime end = new DateTime(2015,5,1);
testdata.ForEach(x=>x.FilterByTime(end));
var testmachine = testdata[0];

List<Row> rows = new List<Row>();
testmachine.Telemetries.ForEach(x =>
{
    var row = new Row() { Parameters = new List<double>() { x.Pressure, x.Rotate, x.Vibration, x.Volt, (x.DateTime - start).TotalHours }, Output = 0 };
    var lastMaint = testmachine.Maints.LastOrDefault(m => m.DateTime < x.DateTime);
    if (lastMaint == null) lastMaint = new Maint() { DateTime = start };
    var nextFail = testmachine.Failures.FirstOrDefault(f => f.DateTime > x.DateTime);
    if (nextFail != null)
    {
        var countErrors = testmachine.Errors.Count(er => er.DateTime > lastMaint.DateTime && er.DateTime < x.DateTime);
        var timeOfLastMaint = (x.DateTime - lastMaint.DateTime).TotalHours;
        var timeOfNextFailt = (nextFail.DateTime - x.DateTime).TotalHours;
        row.Parameters.Add(countErrors);
        row.Parameters.Add(timeOfLastMaint);
        row.Output = timeOfNextFailt;
        rows.Add(row);
    }
});
Tree tree = new Tree(rows);
List<double> realoutputs = rows.Select(x=>x.Output).ToList();
List<double> forecastoutputs = rows.Select(x=>tree.GetForecast(x)).ToList();

var g = "fw";