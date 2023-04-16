using FluentAssertions;
using SRGEnt.Generator.Tests.Utilities;


namespace SRGEnt.Generator.Tests;

public class SRGEnt_Generator_Tests
{
    const string _inputDirectory = "./TestData/Inputs/";
    private const string _outputDirectory = "./TestData/Outputs/";
    
    [Fact]
    public void EmptyGenerationTest()
    {
        var emptyRunGeneratorStats = File.ReadAllText("./TestData/Outputs/EmptyRun_GeneratorStats.cs");
        
        var cdw = CDW.Configure()
            .AddGenerator(new Generator())
            .AddDependency("netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")
            .AddDependency("System.Runtime, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
            .AddSourceFile("")
            .Build();
        
        cdw.Run();

        cdw.Diagnostics.Length.Should().Be(0);
        cdw.OutputCompilation?.SyntaxTrees.Count().Should().Be(2);
        
        var results = cdw.Driver.GetRunResult().Results.FirstOrDefault();
        var generatorStats = results.GeneratedSources.LastOrDefault();

        generatorStats.HintName.Should().Be($"{cdw.OutputCompilation?.AssemblyName}.GeneratorStats.Generated.cs");
        generatorStats.SourceText.ToString().Should().BeEquivalentTo(emptyRunGeneratorStats);
    }

    [Fact]
    public void EmptyEntityAndSystemGeneratorTest()
    {
        var testPathPrefix = "EmptyEntityAndSystem_";
        var source = File.ReadAllText($"{_inputDirectory}{testPathPrefix}Definitions.cs");
        var domain = File.ReadAllText($"{_outputDirectory}{testPathPrefix}Domain.cs");
        var entity = File.ReadAllText($"{_outputDirectory}{testPathPrefix}Entity.cs");
        var matcher = File.ReadAllText($"{_outputDirectory}{testPathPrefix}Matcher.cs");
        var setter = File.ReadAllText($"{_outputDirectory}{testPathPrefix}Setter.cs");

        var cdw = CDW.Configure()
            .AddGenerator(new Generator())
            .AddDependency("netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")
            .AddDependency("System.Runtime, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
            .AddDependency("System.Memory, Version=4.0.1.1, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")
            .AddDependency("SRGEnt.Core, Version=0.5.5.0, Culture=neutral")
            .AddSourceFile(source)
            .Build();
        cdw.Run();
        cdw.Diagnostics.Length.Should().Be(0);
        cdw.OutputCompilation.Should().NotBeNull();
        cdw.OutputCompilation?.SyntaxTrees.Count().Should().Be(8);
        cdw.OutputCompilation?.GetDiagnostics().Length.Should().Be(0);
        var generatedSources = cdw.Driver.GetRunResult().Results.FirstOrDefault().GeneratedSources;

        SourceComparer.CompareTwoFiles(generatedSources[0].SourceText.ToString(),domain,10, $"0 - {generatedSources[0].HintName}");
        SourceComparer.CompareTwoFiles(generatedSources[1].SourceText.ToString(),entity,10, $"1 - {generatedSources[1].HintName}");
        SourceComparer.CompareTwoFiles(generatedSources[2].SourceText.ToString(), setter,10, $"2 - {generatedSources[2].HintName}");
        SourceComparer.CompareTwoFiles(generatedSources[3].SourceText.ToString(), matcher,10, $"3 - {generatedSources[3].HintName}");
    }
}