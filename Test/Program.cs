using DataReader.Types;
using RandomForest;
using Test;

var datas = DataReader.Reader.GetData();
var testdata = datas.Where(x=>x.Model == 1).ToList();

DateTime start = new DateTime(2015, 1, 1);
//DateTime end = new DateTime(2015,6,11);
Data testmachine = testdata[0];
//testmachine.FilterByTime(end);

List<Row> rows = Parser.GetRows(testmachine, start);
//Tree tree = new Tree(rows);
Forest forest = new Forest(rows);

//rows.Clear();


/*testmachine = DataReader.Reader.GetData().Where(x=>x.Model == 1).ToList()[0]; 
rows = Parser.GetRows(testmachine, start);
Tree tree2 = new Tree(rows);*/

var test = testmachine.Telemetries.FirstOrDefault(x => x.DateTime >  new DateTime(2015, 3, 15));
Row row = Parser.GetRow(testmachine, test);
Console.WriteLine("Real date next fail:" + start.AddHours(row.Output));
Console.WriteLine("Forecast date next fail:" + start.AddHours(forest.GetForecast(row)));

test = testmachine.Telemetries.FirstOrDefault(x => x.DateTime >  new DateTime(2015, 1, 25));
 row = Parser.GetRow(testmachine, test);
Console.WriteLine("Real date next fail:" + start.AddHours(row.Output));
Console.WriteLine("Forecast date next fail:" + start.AddHours(forest.GetForecast(row)));

test = testmachine.Telemetries.FirstOrDefault(x => x.DateTime >  new DateTime(2015, 6, 11));
row = Parser.GetRow(testmachine, test);
Console.WriteLine("Real date next fail:" + start.AddHours(row.Output));
Console.WriteLine("Forecast date next fail:" + start.AddHours(forest.GetForecast(row)));

test = testmachine.Telemetries.FirstOrDefault(x => x.DateTime >  new DateTime(2015, 8, 24));
row = Parser.GetRow(testmachine, test);
Console.WriteLine("Real date next fail:" + start.AddHours(row.Output));
Console.WriteLine("Forecast date next fail:" + start.AddHours(forest.GetForecast(row)));


var g = "fw";