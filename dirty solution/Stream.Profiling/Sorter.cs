partial class Sorter
{
    public void Sort(string fileName, int partLinesCount)
    {
        var files = SplitFile(fileName, partLinesCount);
        SortParts(files, partLinesCount);
        SortResult(files);
    }

    private void SortParts(string[] files, int partLinesCount)
    {
        foreach (var file in files)
        {
            var sortedLine = File.ReadAllLines(file)
                                                    .Select(x => new Line(x))
                                                    .OrderBy(x => x);

            File.WriteAllLines(file, sortedLine.Select(x => x.Build()));
        }
    }

    private class LineState
    {
        public StreamReader Reader { get; set; }
        public Line Line { get; set; }
    }
    
    private void SortResult(string[] files)
    {
        var readers = files.Select(x => new StreamReader(x));

        try
        {
            var lines = readers.Select(x => new LineState()
            {
                Line = new Line(x.ReadLine()),
                Reader = x
            }).ToList();

            using var writer = new StreamWriter("result.txt");

            while (lines.Count > 0)
            {
                var current = lines.OrderBy(x => x.Line).First();
                writer.WriteLine(current.Line.Build());
                
                if (current.Reader.EndOfStream)
                {
                    lines.Remove(current);
                    continue;
                }
                
                current.Line = new Line(current.Reader.ReadLine());
            }
        }
        finally
        {
            foreach (var reader in readers)
            {
                reader.Dispose();
            }
        }
    }


    private string[] SplitFile(string fileName, int partLinesCount)
    {
        var list = new List<string>();
        
        using (var reader = new StreamReader(fileName))
        {
            int partNumber = 0;
            
            while (!reader.EndOfStream)
            {
                partNumber++;
                var partFileName = partNumber + ".txt";
                list.Add(partFileName);
                
                using (var writer = new StreamWriter(partFileName))
                {
                    for (int i = 0; i < partLinesCount; i++)
                    {
                        if (reader.EndOfStream)
                            break;
                        writer.WriteLine(reader.ReadLine());
                    }
                }
            }
        }

        return list.ToArray();
    }
}