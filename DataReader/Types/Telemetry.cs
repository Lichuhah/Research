using System.Globalization;

namespace DataReader.Types;

public class Telemetry
{
    public DateTime DateTime { get; set; }   
    public int MachineId { get; set; }
    public double Volt { get; set; }
    public double Rotate { get; set; }
    public double Pressure { get; set; }
    public double Vibration { get; set; }
    
    public static Telemetry Parse(List<string> row)
    {
        return new Telemetry()
        {
            DateTime = Convert.ToDateTime(row[0]),
            MachineId = Convert.ToInt32(row[1]),
            Volt = Convert.ToDouble(row[2], CultureInfo.InvariantCulture),
            Rotate = Convert.ToDouble(row[3], CultureInfo.InvariantCulture),
            Pressure = Convert.ToDouble(row[4], CultureInfo.InvariantCulture),
            Vibration = Convert.ToDouble(row[5], CultureInfo.InvariantCulture),
        };
    }
}