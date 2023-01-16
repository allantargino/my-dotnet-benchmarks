using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Benchmarking;

class Program
{
    static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<HashsetBenchmark>();
    }
}

public class HashsetBenchmark
{
    public class EventTest
    {
        public List<EventId> Events { get; }
        private readonly string caseName;

        public EventTest(List<EventId> events, [CallerArgumentExpression("events")] string caseName = "defaultCase")
        {
            this.Events = events;
            this.caseName = caseName;
        }

        public override string ToString() => caseName;
    }

    public static IEnumerable<EventTest> Events()
    {
        var allEqual = Enumerable.Repeat(new EventId(47), 100).ToList();
        yield return new EventTest(allEqual);

        var _10percEqual = Enumerable.Range(0, 90).Select(i => new EventId(i)).Union(Enumerable.Repeat(new EventId(47), 10)).ToList();
        yield return new EventTest(_10percEqual);

        var _50percEqual = Enumerable.Range(0, 50).Select(i => new EventId(i)).Union(Enumerable.Repeat(new EventId(47), 50)).ToList();
        yield return new EventTest(_50percEqual);

        var allDistinct = Enumerable.Range(0, 100).Select(i => new EventId(i)).ToList();
        yield return new EventTest(allDistinct);
    }


    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Events))]
    public void UsingContainsThenAdd(EventTest eventTest)
    {
        var hashset = new HashSet<int>();

        foreach (var _event in eventTest.Events)
        {
            if (hashset.Contains(_event.Id))
            {
                // do something
            }
            else
            {
                _ = hashset.Add(_event.Id);
            }
        }
    }

    [Benchmark]
    [ArgumentsSource(nameof(Events))]
    public void UsingAdd(EventTest eventTest)
    {
        var hashset = new HashSet<int>();

        foreach (var _event in eventTest.Events)
        {
            if (!hashset.Add(_event.Id))
            {
                // do something
            }
        }
    }
}
