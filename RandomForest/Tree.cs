namespace RandomForest;

public class Tree
{
	public Node Root { get; set; }
	public List<Row> Data { get; set; }
	private int Limit { get; set; }

	public Tree(List<Row> data)
	{
		Random rnd = new Random(DateTime.UtcNow.Microsecond);
		Data = data;
		Limit = rnd.Next(3, 6);
		BuildTree();
	}

	public double GetForecast(Row data)
	{
		return Root.GetNodeForecast(data);
	}

	public int GetCount()
	{
		int count = 0;
		count = Root.GetCount(count);
		return count;
	}

	public double GetMeanRelativeError(List<Row> data)
	{
		double RelError = 0.0; 
		List<double> real = data.Select(x => x.Output).ToList();
		List<double> forecasts = data.Select(x => GetForecast(x)).ToList();
		for (var i = 0; i < real.Count; i++)
		{
			RelError += (real[i] > forecasts[i] ? real[i] / forecasts[i] : forecasts[i] / real[i]);
		}

		return RelError / real.Count();
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
			RelError += (real[i] > forecasts[i] ? real[i] / forecasts[i] : forecasts[i] / real[i]);
		}

		AbsError = AbsError / real.Count;
		RelError = RelError / real.Count;
		stat.SquareError = SquareError;
		stat.MeanRelativeError = RelError;
		stat.MeanAbsoluteError = AbsError;
	}
	
	private void BuildTree()
	{
		Random rnd = new Random(DateTime.UtcNow.Microsecond);
		Root = new Node(Data);
		/*int parNulls = rnd.Next(0, Data[0].Parameters.Count-3);
		for (int i = 0; i < parNulls; i++)
		{
			int parIndex = rnd.Next(Data[0].Parameters.Count - 1);
			Data.ForEach(d =>
			{
				d.Parameters[parIndex] = double.MaxValue;
			});
		}*/
		int depth = 1;
		_BuildTree(Root,depth);
	}
	
	private void _BuildTree(Node root, int depth)
	{
		if(root.IsTerminal) return;
		if(depth == this.Limit) return;
		root.GenerateNodes();
		depth++;
		_BuildTree(root.Right, depth);
		_BuildTree(root.Left, depth);
		depth--;
	}
}