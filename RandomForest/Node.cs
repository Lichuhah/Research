namespace RandomForest;

public class Node
{
    public List<Row> Data { get; set; }
    public Node? Right { get; set; }
    public Node? Left { get; set; }
    private double AllImpurity { get; set; }
    public int ParameterIndex { get; set; }
    public double SeparationValue { get; set; }
    public double Forecast { get; set; }
    public bool IsTerminal { get; set; }

    public Node(List<Row> data)
    {
        Data = data ?? new List<Row>();
        AllImpurity = GetImpurity(data);
        Forecast = GetForecast(data);
        IsTerminal = AllImpurity == 0.0;
    }
    
    private double GetForecast(List<Row> rows)
    {
        double forecast = rows.Select(x => x.Output).Sum() / rows.Count;
        return forecast;
    }
    
    private double GetImpurity(List<Row> rows)
    {
        double forecast = GetForecast(rows);
        return rows.Select(x => Math.Pow(forecast - x.Output, 2)).Sum();
    }

    private double GetInformationGain(List<List<Row>> data)
    {
        double sum = 0.0;
        foreach (List<Row> rows in data)
        {
            if (rows.Count == 0) return 0;
            sum += ((double)rows.Count / Data.Count) * GetImpurity(rows);
        }

        return AllImpurity - sum;
    }
    
    private void GetBestSeparation()
    {
        List<int> valuableParameters = new List<int>();
        for (var i = 0; i < Data[0].Parameters.Count; i++)
        {
            if(Data[0].Parameters[i] != -1) valuableParameters.Add(i);
        }
        int imagesCount = Data.Count;
        double bestInformationGain = 0.0;
        
        foreach (var valuableParameterIndex in valuableParameters)
        {
            List<double> parameters = Data.Select(x => x.Parameters[valuableParameterIndex]).ToList();
            for (int j = 0; j < imagesCount; j++)
            {
                double gain = GetInformationGain(new List<List<Row>>()
                {
                    Data.Where(x => x.Parameters[valuableParameterIndex] <= parameters[j]).ToList(),
                    Data.Where(x => x.Parameters[valuableParameterIndex] > parameters[j]).ToList()
                });
                if (gain > bestInformationGain)
                {
                    bestInformationGain = gain;
                    ParameterIndex = valuableParameterIndex;
                    SeparationValue = parameters[j];
                }
            }
        }

        if (bestInformationGain == 0)
        {
            IsTerminal = true;
        }
    }

    public void GenerateNodes()
    {
        GetBestSeparation();
        if (!IsTerminal)
        {
            Left = new Node(Data.Where(x => x.Parameters[ParameterIndex] <= SeparationValue).ToList());
            Right = new Node(Data.Where(x => x.Parameters[ParameterIndex] > SeparationValue).ToList());
        }
        else
        {
            Left = null;
            Right = null;
        }
    }

    public double GetNodeForecast(Row row)
    {
        if (IsTerminal) return Forecast;
        if (row.Parameters[ParameterIndex] <= SeparationValue) return Left?.GetNodeForecast(row) ?? Forecast;
        else return Right?.GetNodeForecast(row) ?? Forecast;
    }

    public int GetCount(int count)
    {
        count++;
        if (Right != null) count = Right.GetCount(count);
        if (Left != null) count = Left.GetCount(count);
        return count;
    }
}