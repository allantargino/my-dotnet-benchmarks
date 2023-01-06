``` ini

BenchmarkDotNet=v0.13.3, OS=Windows 11 (10.0.22623.1095)
Intel Core i7-8665U CPU 1.90GHz (Coffee Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100
  [Host]     : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2


```
|               Method |    eventTest |       Mean |    Error |   StdDev | Ratio | RatioSD |
|--------------------- |------------- |-----------:|---------:|---------:|------:|--------:|
| **UsingContainsThenAdd** | **_10percEqual** | **2,166.0 ns** | **43.21 ns** | **60.58 ns** |  **1.00** |    **0.00** |
|             UsingAdd | _10percEqual | 1,602.6 ns | 32.07 ns | 44.96 ns |  0.74 |    0.03 |
|                      |              |            |          |          |       |         |
| **UsingContainsThenAdd** | **_50percEqual** | **1,060.7 ns** | **20.91 ns** | **37.17 ns** |  **1.00** |    **0.00** |
|             UsingAdd | _50percEqual |   792.2 ns | 15.55 ns | 16.63 ns |  0.74 |    0.02 |
|                      |              |            |          |          |       |         |
| **UsingContainsThenAdd** |  **allDistinct** | **2,062.3 ns** | **34.20 ns** | **31.99 ns** |  **1.00** |    **0.00** |
|             UsingAdd |  allDistinct | 1,677.0 ns | 30.36 ns | 28.40 ns |  0.81 |    0.02 |
|                      |              |            |          |          |       |         |
| **UsingContainsThenAdd** |     **allEqual** |   **752.4 ns** | **14.90 ns** | **19.38 ns** |  **1.00** |    **0.00** |
|             UsingAdd |     allEqual |   689.0 ns | 13.72 ns | 14.09 ns |  0.91 |    0.03 |
