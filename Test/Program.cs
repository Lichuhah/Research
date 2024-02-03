using DataReader.Types;
using RandomForest;
using Test;

var datas = DataReader.Reader.GetData();

DateTime start = new DateTime(2015, 1, 1);
DateTime end = new DateTime(2015,3,1);
datas = datas.Where(x=>x.Model == 1).ToList();
datas.ForEach(x => x.FilterByTime(end));

List<Row> rows = Parser.GetRowsForType(datas, start);
//Tree tree = new Tree(rows);
Forest forest = new Forest(rows);

//rows.Clear();


/*testmachine = DataReader.Reader.GetData().Where(x=>x.Model == 1).ToList()[0]; 
rows = Parser.GetRows(testmachine, start);
Tree tree2 = new Tree(rows);*/

var test = datas[0].Telemetries[200];
Row row = Parser.GetRow(datas[0], test);
Console.WriteLine("Real date next fail:" + start.AddHours(row.Output));
Console.WriteLine("Forecast date next fail:" + start.AddHours(forest.GetForecast(row)));

test = datas[3].Telemetries[400];
 row = Parser.GetRow(datas[3], test);
Console.WriteLine("Real date next fail:" + start.AddHours(row.Output));
Console.WriteLine("Forecast date next fail:" + start.AddHours(forest.GetForecast(row)));

test = datas[6].Telemetries[600];
row = Parser.GetRow(datas[6], test);
Console.WriteLine("Real date next fail:" + start.AddHours(row.Output));
Console.WriteLine("Forecast date next fail:" + start.AddHours(forest.GetForecast(row)));

test = datas[5].Telemetries[800];
row = Parser.GetRow(datas[5], test);
Console.WriteLine("Real date next fail:" + start.AddHours(row.Output));
Console.WriteLine("Forecast date next fail:" + start.AddHours(forest.GetForecast(row)));


var g = "fw";