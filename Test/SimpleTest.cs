using RandomForest;

namespace Test;

public static class SimpleTest
{
    public static void Start()
    {
        List<Row> rows = new List<Row>()
        {
            new Row() { Parameters = new List<double>() { 1.0, 0.3 }, Output = 1 },
            new Row() { Parameters = new List<double>() { 2.0, 0.6 }, Output = 1 },
            new Row() { Parameters = new List<double>() { 3.0, 0.2 }, Output = 1 },
            new Row() { Parameters = new List<double>() { 5.3, 0.1 }, Output = 1 },
            new Row() { Parameters = new List<double>() { 4.0, 0.4 }, Output = 1 },
            new Row() { Parameters = new List<double>() { 6.5, 0.33 }, Output = 1 },

            new Row() { Parameters = new List<double>() { 3.0, 1.0 }, Output = 2 },
            new Row() { Parameters = new List<double>() { 4.4, 1.4 }, Output = 2 },
            new Row() { Parameters = new List<double>() { 2.5, 1.5 }, Output = 2 },
            new Row() { Parameters = new List<double>() { 1.1, 1.1 }, Output = 2 },
            new Row() { Parameters = new List<double>() { 3.2, 1.2 }, Output = 2 },
            new Row() { Parameters = new List<double>() { 1.6, 1.3 }, Output = 2 },
            new Row() { Parameters = new List<double>() { 5.4, 1.3 }, Output = 2 },

            new Row() { Parameters = new List<double>() { 6.4, 1.4 }, Output = 3 },
            new Row() { Parameters = new List<double>() { 6.3, 1.3 }, Output = 3 },
            new Row() { Parameters = new List<double>() { 5.5, 1.5 }, Output = 3 },
            new Row() { Parameters = new List<double>() { 7.6, 1.6 }, Output = 3 },
            new Row() { Parameters = new List<double>() { 6.7, 1.7 }, Output = 3 },
            new Row() { Parameters = new List<double>() { 5.1, 1.5 }, Output = 3 },
        };

        Tree tree = new Tree(rows);
        Console.WriteLine(tree.GetForecast(new Row() { Parameters = new List<double>() { 6.4, 0.2 }, Output = 1 }));
        Console.WriteLine(tree.GetForecast(new Row() { Parameters = new List<double>() { 1, 0.4 }, Output = 1 }));

        Console.WriteLine(tree.GetForecast(new Row() { Parameters = new List<double>() { 4.0, 1.4 }, Output = 2 }));
        Console.WriteLine(tree.GetForecast(new Row() { Parameters = new List<double>() { 5.4, 1.2 }, Output = 2 }));

        Console.WriteLine(tree.GetForecast(new Row() { Parameters = new List<double>() { 5.7, 1.5 }, Output = 3 }));
        Console.WriteLine(tree.GetForecast(new Row() { Parameters = new List<double>() { 6.2, 1.8 }, Output = 3 }));
    }
}