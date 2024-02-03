using System.Diagnostics;

namespace RandomForest;

public static class Timer
{
    public static Stopwatch Start(string str)
    {
        Console.WriteLine("Start " + str);
        return System.Diagnostics.Stopwatch.StartNew();
    }
    
    public static long End(this Stopwatch timer)
    {
        long time = timer.ElapsedMilliseconds;
        timer.Stop();
        Console.WriteLine($"Total Execution Time: {timer.ElapsedMilliseconds} ms");
        timer.Reset();
        return time;
    }
}