using System.Text;

namespace SRGEnt.Generator.Tests.Utilities;

public class SourceComparer
{
    public static void CompareTwoFiles(string first, string second, int bufferLength = 5, string errorHeader = "")
    {
        if (bufferLength < 2) bufferLength = 2;
        
        first = FormattedFileWriter.FormatSource(first);
        second = FormattedFileWriter.FormatSource(second);
        
        var sb = new StringBuilder();
        sb.AppendLine(errorHeader);
        
        var firstStream = GenerateStreamFromString(first);
        var secondStream = GenerateStreamFromString(second);

        var circularBuffer = new string[bufferLength];
        var bufferIndex = 0;
        
        var firstStreamReader = new StreamReader(firstStream);
        var secondStreamReader = new StreamReader(secondStream);

        var linesRead = 0;
        while (!firstStreamReader.EndOfStream && !secondStreamReader.EndOfStream)
        {
            var firstLine = firstStreamReader.ReadLine();
            var secondLine = secondStreamReader.ReadLine();
            linesRead++;
            
            circularBuffer[bufferIndex % bufferLength] = firstLine;
            bufferIndex++;
            
            if (string.CompareOrdinal(firstLine, secondLine) != 0)
            {
                sb.AppendLine("The files were not identical.");
                sb.AppendLine($"The difference appeared on line {linesRead}");
                if (bufferIndex < bufferLength)
                {
                    for (var i = 0; i < bufferIndex; i++)
                    {
                        var lineNumber = i + 1;
                        sb.AppendLine($"{lineNumber}\t{circularBuffer[i]}");
                    }
                    sb.AppendLine($"ErrorLine");
                    sb.AppendLine($"{bufferIndex} {secondLine}");
                }
                else
                {
                    var lineNumber = linesRead - bufferLength + 1;
                    for (var i = bufferIndex - bufferLength; i < bufferIndex; i++)
                    {
                        sb.AppendLine($"{lineNumber}\t{circularBuffer[i % bufferLength]}");
                        lineNumber++;
                    }
                    sb.AppendLine($"---------------");
                    sb.AppendLine($"{lineNumber - 1}\t{secondLine}");
                }
                Assert.Fail(sb.ToString());
            }
        }

        if (!firstStreamReader.EndOfStream && secondStreamReader.EndOfStream)
        {
            Assert.Fail($"{errorHeader}\nSecond File Ended Before the first one after reading {linesRead} lines.");
        }

        if (firstStreamReader.EndOfStream && !secondStreamReader.EndOfStream)
        {
            Assert.Fail($"{errorHeader}First File Ended Before the second one after reading {linesRead} lines");
        }
    }
    
    private static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}