namespace RandomForest;

public class Forest
{
    public List<Row> Data { get; set; }
    public List<Tree> Trees { get; set; }
    public Stat Statistic { get; set; }

    private int MAX_TREE = 3000;
    private int MAX_DEPTH = 100;
    private int BAG_SIZE = 300;
    private int COUNT_PARAMETERS = 100;
    
    private Random rnd = new Random(DateTime.UtcNow.Microsecond);

    public Forest(List<Row> data)
    {
        Data = data;
        Trees = new List<Tree>();
        var timer = Timer.Start("Start build tree");
        COUNT_PARAMETERS = data[0].Parameters.Count / 3;
        BAG_SIZE /= 10;
        InitForest();
        Statistic = new Stat()
        {
            Time = timer.End(),
            CountTree = Trees.Count
        };
        //SetError(Statistic);
    }
    
    public double GetForecast(Row data)
    {
        List<double> results = Trees.Select(x => x.GetForecast(data)).ToList();
        return results.Sum() / results.Count;
    }
    
    public double GetForecast(Row data, List<Tree> trees)
    {
        List<double> results = trees.Select(x => x.GetForecast(data)).ToList();
        return results.Sum() / results.Count;
    }

    public void InitForest()
    {
        //Trees.Add(new Tree(GetRandomSet(), MAX_DEPTH));
   
        double relErr = int.MaxValue;
        double prevErr = int.MaxValue;

        while (Trees.Count < MAX_TREE)
        {
            Console.WriteLine("______________");
            Console.WriteLine("Count trees:" + Trees.Count);
            Console.WriteLine("Square error:" + prevErr);
            List<Tree> newTrees = new List<Tree>();
            Task[] tasks = new Task[30];
            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new Task(() =>
                {
                    List<Row> set = GetRandomSet();
                    var tree = new Tree(set, MAX_DEPTH);
                    newTrees.Add(tree);
                });
                tasks[i].Start(); // запускаем задачу
            }

            Task.WaitAll(tasks);
            newTrees.ForEach(x =>
            {
                Trees.Add(x);
                /*var testTree = new List<Tree>(Trees) { x };
                double newEr = GetSquareError(testTree);
                if (newEr <= prevErr) Trees.Add(x);*/
            });
            prevErr = GetSquareError(Trees);
            InitTest();
        }
    }

    public void InitTest()
    {
        DateTime start = new DateTime(2015, 1, 1);
        Row row = Data[rnd.Next(Data.Count-1)];
        Console.WriteLine("real out: " + start.AddDays(row.Output));
        Console.WriteLine("forest out:" + start.AddDays(GetForecast(row)));
    }
    
    public List<Row> GetRandomSet()
    {
        List<Row> data = GetProblems();
        List<Row> set = new List<Row>();
        data.ForEach(x =>
        {
            set.Add(new Row()
            {
                Output = x.Output,
                Parameters = new List<double>(x.Parameters)
            });
        });
        for (int i = 0; i < BAG_SIZE/2; i++)
        {
            int index = rnd.Next(Data.Count - 1);
            set.Add(new Row()
            {
                Output = Data[index].Output,
                Parameters = new List<double>(Data[index].Parameters)
            });
        }

        Console.WriteLine(string.Concat(set.Select(x=>x.Output+",")));
        return SetNullParameters(set);
    }

    public List<Row> GetProblems()
    {
        if (this.Trees.Count > 0)
        {
            List<int> real = Data.Select(x => (int)x.Output).ToList();
            List<int> forecasts = Data.Select(x => (int)GetForecast(x, Trees)).ToList();
            Dictionary<int, double> dict = new Dictionary<int, double>();
            for (var i = 0; i < real.Count; i++)
            {
                dict.Add(i, Math.Pow(real[i] - forecasts[i], 2));
            }

            dict = dict.OrderByDescending(x => x.Value).Take(BAG_SIZE / 2).ToDictionary(x => x.Key, x => x.Value);
            List<Row> rows = new List<Row>();
            foreach (var keyValuePair in dict)
            {
                rows.Add(Data[keyValuePair.Key]);
            }

            return rows;
        }
        else
        {
            List<Row> rows = this.Data.OrderBy(x => x.Output).Take(BAG_SIZE / 4).ToList();
            rows.AddRange(this.Data.OrderByDescending(x => x.Output).Take(BAG_SIZE / 4).ToList());
            return rows;
        }
    }

    public List<Row> SetNullParameters(List<Row> set)
    {
        List<int> paramsIndexes = new List<int>();
        for (int i = 0; i < set[0].Parameters.Count; i++)
        {
            paramsIndexes.Add(i);
        }
        for (int i = set[0].Parameters.Count; i > COUNT_PARAMETERS; i--)
        {
            int offParamIndex = paramsIndexes[rnd.Next(paramsIndexes.Count-1)];
            paramsIndexes.Remove(offParamIndex);
            set.ForEach(x=>x.Parameters[offParamIndex] = -1);
        }
        return set;
    }

    public double GetMeanRelativeError(List<Tree> tree)
    {
        double RelError = 0.0; 
        List<int> real = Data.Select(x => (int)x.Output).ToList();
        List<int> forecasts = Data.Select(x => (int)GetForecast(x,tree)).ToList();
        for (var i = 0; i < real.Count; i++)
        {
            RelError += (real[i] > forecasts[i] ? forecasts[i] / real[i] : real[i] / forecasts[i]);
        }

        if (RelError == 0) return 0;
        Console.WriteLine(RelError/forecasts.Count);
        return RelError/forecasts.Count;
    }
    
    public double GetSquareError(List<Tree> tree)
    {
        double SquareError = 0.0; 
        List<int> real = Data.Select(x => (int)x.Output).ToList();
        List<int> forecasts = Data.Select(x => (int)GetForecast(x,tree)).ToList();
        for (var i = 0; i < real.Count; i++)
        {
            SquareError += Math.Pow(real[i] - forecasts[i],2);
        }
        
        return SquareError;
    }
    
    public void SetError(Stat stat)
    {
        double SquareError = 0.0; 
        double AbsError = 0.0; 
        double RelError = 0.0; 
        List<double> real = Data.Select(x => x.Output).ToList();
        List<double> forecasts = Data.Select(x => GetForecast(x)).ToList();
        for (var i = 0; i < real.Count; i++)
        {
            SquareError += Math.Pow(real[i] - forecasts[i], 2);
            AbsError += Math.Abs(real[i] - forecasts[i]);
            RelError += (real[i] > forecasts[i] ? forecasts[i] / real[i] : real[i] / forecasts[i]);
        }

        AbsError = AbsError / real.Count;
        RelError = RelError / real.Count;
        stat.SquareError = SquareError;
        stat.MeanRelativeError = RelError;
        stat.MeanAbsoluteError = AbsError;
    }
}