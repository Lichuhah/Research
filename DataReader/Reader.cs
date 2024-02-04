using DataReader.Types;

namespace DataReader;

public class Reader
{
    private static List<List<string>> ReadFile(string name)
    {
        using(var reader = new StreamReader("./Data/"+name+".csv"))
        {
            List<List<string>> list = new List<List<string>>();
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                list.Add(values.ToList());
            }

            return list;
        }
    }

    private static List<Error> ReadError()
    {
        List<List<string>> str = ReadFile("PdM_errors");
        List<Error> errors = str.Select(x => Error.Parse(x)).ToList();
        return errors;
    }
    
    
    private static List<Failure> ReadFailures()
    {
        List<List<string>> str = ReadFile("PdM_failures");
        List<Failure> errors = str.Select(x => Failure.Parse(x)).ToList();
        return errors;
    }
    
    
    private static List<Machine> ReadMachines()
    {
        List<List<string>> str = ReadFile("PdM_machines");
        List<Machine> errors = str.Select(x => Machine.Parse(x)).ToList();
        return errors;
    }
    
    
    private static List<Maint> ReadMaints()
    {
        List<List<string>> str = ReadFile("PdM_maint");
        List<Maint> errors = str.Select(x => Maint.Parse(x)).ToList();
        return errors;
    }
    
    
    private static List<Telemetry> ReadTelemetry()
    {
        List<List<string>> str = ReadFile("PdM_telemetry");
        List<Telemetry> errors = str.Select(x => Telemetry.Parse(x)).ToList();
        return errors;
    }

    public static List<Data> GetData()
    {
        var errors = ReadError();
        var failures = ReadFailures();
        var machines = ReadMachines();
        var maint = ReadMaints();
        var telemetries = ReadTelemetry();

        return machines.Select(m => new Data()
        {
            Model = m.Model,
            Id = m.MachineId,
            Age = m.Age,
            Errors = errors.Where(x => x.MachineId == m.MachineId).OrderBy(x=>x.DateTime).ToList(),
            Failures = failures.Where(x => x.MachineId == m.MachineId).OrderBy(x=>x.DateTime).ToList(),
            Maints = maint.Where(x => x.MachineId == m.MachineId).OrderBy(x=>x.DateTime).ToList(),
            _Telemetries = telemetries.Where(x => x.MachineId == m.MachineId).OrderBy(x=>x.DateTime).ToList(),
        }).ToList();
    }
}