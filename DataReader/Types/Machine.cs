namespace DataReader.Types;

public class Machine
{
    public int MachineId { get; set; }
    public int Model { get; set; }
    public int Age { get; set; }
    
    public static Machine Parse(List<string> row)
    {
        return new Machine()
        {
            MachineId = Convert.ToInt32(row[0]),
            Model =  Convert.ToInt32(row[1].Substring(6,1)),
            Age = Convert.ToInt32(row[2])
        };
    }
}