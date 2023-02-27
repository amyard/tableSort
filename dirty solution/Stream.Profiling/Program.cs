

using Stream.Profiling;

var fileName = new Generator().Generate(10000);
new Sorter().Sort(fileName, 1000);