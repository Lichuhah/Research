namespace DataReader.Types;

public class Error
{
    public DateTime DateTime { get; set; }
    public int MachineId { get; set; }
    public int ErrorId { get; set; }

    public static Error Parse(List<string> row)
    {
        return new Error()
        {
            DateTime = Convert.ToDateTime(row[0]),
            MachineId = Convert.ToInt32(row[1]),
            ErrorId = Convert.ToInt32(row[2].Substring(6,1))
        };
    }
}