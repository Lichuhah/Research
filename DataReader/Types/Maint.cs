namespace DataReader.Types;

public class Maint
{
    public DateTime DateTime { get; set; }
    public int MachineId { get; set; }
    public int FailureId { get; set; }
    
    public static Maint Parse(List<string> row)
    {
        return new Maint()
        {
            DateTime = Convert.ToDateTime(row[0]),
            MachineId = Convert.ToInt32(row[1]),
            FailureId = Convert.ToInt32(row[2].Substring(5,1))
        };
    }
}