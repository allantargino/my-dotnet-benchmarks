# Case Sensitive Parameters

> Work related to the issue [[LSG] LoggerMessage - Add diagnostic - Can't have the same template with different casing](https://github.com/dotnet/runtime/issues/52228).
> 
Here I compare different algorithms (essencially LINQ vs manual grouping). The conclusion is that, while I can perform better by using the manual approach,
LINQ syntax is way cleaner. Giving this is going to run in compile time, I'd say 100ns-200ns is worth losing in favor of readable code.

```ini
BenchmarkDotNet=v0.13.3, OS=Windows 11 (10.0.22623.1095)
Intel Core i7-8665U CPU 1.90GHz (Coffee Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=8.0.100-alpha.1.23061.8
  [Host]     : .NET 7.0.2 (7.0.222.60605), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.2 (7.0.222.60605), X64 RyuJIT AVX2
```

| Method                     | EntryList                          |     Mean |    Error |   StdDev | Ratio | RatioSD |   Gen0 | Allocated | Alloc Ratio |
|----------------------------|------------------------------------|---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| GroupByWithWhere           | ["p1", "p1", "P1", "p1", "a", "b"] | 554.1 ns | 10.30 ns |  9.63 ns |  1.00 |    0.00 | 0.1698 |     712 B |        1.00 |
| GroupByWithCountInside     | ["p1", "p1", "P1", "p1", "a", "b"] | 438.3 ns |  8.55 ns | 12.79 ns |  0.80 |    0.03 | 0.1564 |     656 B |        0.92 |
| GroupByWithNoCount         | ["p1", "p1", "P1", "p1", "a", "b"] | 481.9 ns |  9.43 ns | 12.90 ns |  0.87 |    0.03 | 0.1755 |     736 B |        1.03 |
| ManuallyGroup              | ["p1", "p1", "P1", "p1", "a", "b"] | 339.5 ns |  6.06 ns |  7.44 ns |  0.61 |    0.02 | 0.1144 |     480 B |        0.67 |
| ManuallyGroupTryGet        | ["p1", "p1", "P1", "p1", "a", "b"] | 269.6 ns |  5.40 ns |  7.39 ns |  0.49 |    0.01 | 0.1144 |     480 B |        0.67 |
| ManuallyGroupTryGetHashset | ["p1", "p1", "P1", "p1", "a", "b"] | 392.6 ns |  3.74 ns |  3.12 ns |  0.71 |    0.01 | 0.1779 |     744 B |        1.04 |
|                            |                                    |          |          |          |       |         |        |           |             |
| GroupByWithWhere           | ["p1", "a", "b"]                   | 314.5 ns |  6.22 ns |  9.86 ns |  0.57 |    0.02 | 0.1373 |     576 B |        0.81 |
| GroupByWithCountInside     | ["p1", "a", "b"]                   | 268.2 ns |  5.40 ns |  8.87 ns |  0.49 |    0.02 | 0.1240 |     520 B |        0.73 |
| GroupByWithNoCount         | ["p1", "a", "b"]                   | 307.5 ns |  5.51 ns |  5.15 ns |  0.56 |    0.02 | 0.1526 |     640 B |        0.90 |
| ManuallyGroup              | ["p1", "a", "b"]                   | 242.7 ns |  4.09 ns |  3.19 ns |  0.44 |    0.01 | 0.1144 |     480 B |        0.67 |
| ManuallyGroupTryGet        | ["p1", "a", "b"]                   | 206.4 ns |  4.01 ns |  4.77 ns |  0.37 |    0.01 | 0.1147 |     480 B |        0.67 |
| ManuallyGroupTryGetHashset | ["p1", "a", "b"]                   | 312.5 ns |  5.51 ns |  8.75 ns |  0.57 |    0.02 | 0.1779 |     744 B |        1.04 |
