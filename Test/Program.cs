using DataReader.Types;
using RandomForest;
using Test;

var datas = DataReader.Reader.GetData();

DateTime start = new DateTime(2015, 2, 1);
DateTime end = new DateTime(2015,5,1);
datas = datas.Where(x=>x.Model == 3).Take(1).ToList();

datas = datas.Where(x => x.Failures.Count > 0).ToList();
datas.ForEach(x => x.FilterBeforeLastFailure(end));
datas.ForEach(x => x.InitParameters());

List<Row> rows = Parser.GetRowsForType(datas, start);
//Tree tree = new Tree(rows);
Forest forest = new Forest(rows);


var g = "fw";