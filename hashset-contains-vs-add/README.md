# HashSet Contains vs Add

I was trying to decide if I should refactor [LoggerMessageGenerator.Parser.cs](https://github.com/dotnet/runtime/pull/80460/files)
to use `HashSet.Add` only instead of `HashSet.Contains`+`HashSet.Add`. This benchmark proved that using only `HashSet.Add` was more efficient.

```ini
BenchmarkDotNet=v0.13.3, OS=Windows 11 (10.0.22623.1095)
Intel Core i7-8665U CPU 1.90GHz (Coffee Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100
  [Host]     : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2
```

|               Method |    eventTest |       Mean |    Error |   StdDev | Ratio | RatioSD |
|--------------------- |------------- |-----------:|---------:|---------:|------:|--------:|
| UsingContainsThenAdd | _10percEqual | 2,259.5 ns | 38.68 ns | 36.18 ns |  1.00 |    0.00 |
|             UsingAdd | _10percEqual | 1,584.1 ns | 31.58 ns | 36.37 ns |  0.70 |    0.02 |
|                      |              |            |          |          |       |         |
| UsingContainsThenAdd | _50percEqual | 1,045.1 ns | 20.18 ns | 26.94 ns |  1.00 |    0.00 |
|             UsingAdd | _50percEqual |   806.2 ns | 15.87 ns | 21.72 ns |  0.77 |    0.03 |
|                      |              |            |          |          |       |         |
| UsingContainsThenAdd |  allDistinct | 2,082.8 ns | 41.40 ns | 53.83 ns |  1.00 |    0.00 |
|             UsingAdd |  allDistinct | 1,670.1 ns | 20.86 ns | 47.51 ns |  0.81 |    0.04 |
|                      |              |            |          |          |       |         |
| UsingContainsThenAdd |     allEqual |   775.7 ns | 15.49 ns | 29.48 ns |  1.00 |    0.00 |
|             UsingAdd |     allEqual |   660.8 ns | 13.08 ns | 18.33 ns |  0.85 |    0.03 |
