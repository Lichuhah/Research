namespace RandomForest;

public class Forest
{
    public List<Row> Data { get; set; }
    public List<Tree> Trees { get; set; }
    public Stat Statistic { get; set; }

    public Forest(List<Row> data)
    {
        this.Data = data;
        Trees = new List<Tree>();
        var timer = Timer.Start("Start build tree");
        InitForest();
        Statistic = new Stat()
        {
            Time = timer.End(),
            CountTree = Trees.Count
        };
        SetError(Statistic);
    }
    
    public double GetForecast(Row data)
    {
        List<double> results = this.Trees.Select(x => x.GetForecast(data)).ToList();
        return results.Sum() / results.Count;
    }

    public void InitForest()
    {
        Trees.Add(new Tree(GetRandomSet(), 5));
        while (GetMeanRelativeError() < 0.90 && Trees.Count<150)
        {
            this.Trees.Add(new Tree(GetRandomSet(), 5));
        }
    }

    public List<Row> GetRandomSet()
    {
        Random rnd = new Random(DateTime.UtcNow.Microsecond);
        List<Row> data = new List<Row>(Data);
        List<Row> set = new List<Row>();
        int count = rnd.Next(Data.Count/6, Data.Count / 3);
        for (int i = 0; i < count; i++)
        {
            int index = rnd.Next(data.Count - 1);
            set.Add(data[index]);
            data.RemoveAt(index);
        }

        return set;
    }

    public double GetMeanRelativeError()
    {
        double RelError = 0.0; 
        List<double> real = Data.Select(x => x.Output).ToList();
        List<double> forecasts = Data.Select(x => GetForecast(x)).ToList();
        for (var i = 0; i < real.Count; i++)
        {
            RelError += (real[i] > forecasts[i] ? forecasts[i] / real[i] : real[i] / forecasts[i]);
        }

        if (RelError == 0) return 0;
        Console.WriteLine(RelError/forecasts.Count);
        return RelError/forecasts.Count;
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