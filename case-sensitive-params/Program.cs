using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Benchmarking;

class Program
{
    static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<CaseDetectorBenchmark>();
    }
}

[MemoryDiagnoser]
public class CaseDetectorBenchmark
{
    [ParamsSource(nameof(entryList))]
    public List<string> EntryList { get; set; }

    public IEnumerable<List<string>> entryList => new[] {
        new List<string>() { "p1", "p1", "P1", "p1", "a", "b" },
        new List<string>() { "p1", "a", "b" }
    };

    [Benchmark(Baseline = true)]
    public void GroupByWithWhere()
    {
        IEnumerable<IGrouping<string, string>> parametersGroup = EntryList
            .GroupBy(s => s, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1);

        foreach (IGrouping<string, string> parameters in parametersGroup)
        {
            foreach (var parameter in parameters)
            {
                if (!parameters.Key.Equals(parameter, StringComparison.Ordinal))
                {
                    break;
                }
            }
        }
    }

    [Benchmark]
    public void GroupByWithCountInside()
    {
        IEnumerable<IGrouping<string, string>> parametersGroup = EntryList
            .GroupBy(s => s, StringComparer.OrdinalIgnoreCase);

        foreach (IGrouping<string, string> parameters in parametersGroup)
        {
            if (parameters.Count() > 1)
            {
                foreach (var parameter in parameters)
                {
                    if (!parameters.Key.Equals(parameter, StringComparison.Ordinal))
                    {
                        break;
                    }
                }
            }
        }
    }

    [Benchmark]
    public void GroupByWithNoCount()
    {
        IEnumerable<IGrouping<string, string>> parametersGroup = EntryList
            .GroupBy(s => s, StringComparer.OrdinalIgnoreCase);

        foreach (IGrouping<string, string> parameters in parametersGroup)
        {
            foreach (var parameter in parameters)
            {
                if (!parameters.Key.Equals(parameter, StringComparison.Ordinal))
                {
                    break;
                }
            }
        }
    }

    [Benchmark]
    public void ManuallyGroup()
    {
        var dictionary = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in EntryList)
        {
            if (!dictionary.ContainsKey(item))
            {
                dictionary[item] = new List<string>();
            }

            dictionary[item].Add(item);
        }

        foreach (var kv in dictionary)
        {
            if (kv.Value.Count > 1)
            {
                string first = kv.Value[0];
                for (int j = 1; j < kv.Value.Count; j++)
                {
                    if (!first.Equals(kv.Value[j], StringComparison.Ordinal))
                    {
                        break;
                    }
                }
            }
        }
    }

    [Benchmark]
    public void ManuallyGroupTryGet()
    {
        var dictionary = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in EntryList)
        {
            if (!dictionary.TryGetValue(item, out var list))
            {
                list = dictionary[item] = new List<string>();
            }

            list.Add(item);
        }

        foreach (var kv in dictionary)
        {
            if (kv.Value.Count > 1)
            {
                string first = kv.Value[0];
                for (int j = 1; j < kv.Value.Count; j++)
                {
                    if (!first.Equals(kv.Value[j], StringComparison.Ordinal))
                    {
                        break;
                    }
                }
            }
        }
    }

    [Benchmark]
    public void ManuallyGroupTryGetHashset()
    {
        var dictionary = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in EntryList)
        {
            if (!dictionary.TryGetValue(item, out var list))
            {
                list = dictionary[item] = new HashSet<string>();
            }

            list.Add(item);
        }

        foreach (var kv in dictionary)
        {
            foreach (var item in kv.Value)
            {
                if (!kv.Key.Equals(item, StringComparison.Ordinal))
                {
                    break;
                }
            }
        }
    }
}
