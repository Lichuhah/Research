namespace RandomForest;

public class Tree
{
	public Node Root { get; set; }
	public List<Row> Data { get; set; }
	private int Limit { get; set; }

	public Tree(List<Row> data, int limit=10)
	{
		Data = data;
		this.Limit = limit;
		BuildTree();
	}

	public double GetForecast(Row data)
	{
		return Root.GetNodeForecast(data);
	}
	
	private void BuildTree()
	{
		Root = new Node(Data);
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