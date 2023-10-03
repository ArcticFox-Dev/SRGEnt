using System.Text;
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
        const string assemblyName = "EmptyRun";
        var emptyRunGeneratorStats = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_GeneratorStats.cs");
        
        var cdw = CDW.Configure()
            .AddGenerator(new Generator())
            .AddDependency("netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")
            .AddDependency("System.Runtime, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
            .AddSourceFile("")
            .SetAssemblyName("EmptyRun")
            .Build();
        
        cdw.Run();

        cdw.Diagnostics.Length.Should().Be(0);
        cdw.OutputCompilation?.SyntaxTrees.Count().Should().Be(2);

        var sourceComparer = new SourceComparer(cdw);
        
        sourceComparer.CompareGeneratedToSourceString($"{assemblyName}.GeneratorStats.Generated.cs",emptyRunGeneratorStats);
    }

    [Fact]
    public void EmptyEntityAndSystemGeneratorTest()
    {
        const string assemblyName = "EmptyEntityAndSystem";
        
        var source = File.ReadAllText($"{_inputDirectory}/{assemblyName}/{assemblyName}_Definitions.cs");
        var domain = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_Domain.cs");
        var entity = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_Entity.cs");
        var matcher = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_Matcher.cs");
        var setter = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_Setter.cs");
        var executeSystem = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_ExecuteSystem.cs");
        var reactiveSystem = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_ReactiveSystem.cs");
        var generatorStats = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_GeneratorStats.cs");

        var cdw = CDW.Configure()
            .AddGenerator(new Generator())
            .AddDependency("netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")
            .AddDependency("System.Runtime, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
            .AddDependency("System.Memory, Version=4.0.1.1, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")
            .AddDependency("SRGEnt.Core, Version=0.5.5.0, Culture=neutral")
            .AddSourceFile(source)
            .SetAssemblyName("EmptyEntityAndSystem")
            .Build();
        cdw.Run();
        cdw.Diagnostics.Length.Should().Be(0);
        cdw.OutputCompilation.Should().NotBeNull();
        cdw.OutputCompilation?.SyntaxTrees.Count().Should().Be(8);
        cdw.OutputCompilation?.GetDiagnostics().Length.Should().Be(0);

        var sourceComparer = new SourceComparer(cdw);
        sourceComparer.CompareGeneratedToSourceString("TestDomain.Generated.cs",domain);
        sourceComparer.CompareGeneratedToSourceString("TestEntity.Generated.cs",entity);
        sourceComparer.CompareGeneratedToSourceString("TestAspectSetter.Generated.cs",setter);
        sourceComparer.CompareGeneratedToSourceString("TestMatcher.Generated.cs",matcher);
        sourceComparer.CompareGeneratedToSourceString("TestExecuteSystem.Generated.cs",executeSystem);
        sourceComparer.CompareGeneratedToSourceString("TestReactiveSystem.Generated.cs",reactiveSystem);
        sourceComparer.CompareGeneratedToSourceString($"{assemblyName}.GeneratorStats.Generated.cs",generatorStats);
    }
    
    [Fact]
    public void OneComponentEntityGeneratorTest()
    {
        const string assemblyName = "SingleComponentEntity";
        
        var source = File.ReadAllText($"{_inputDirectory}/{assemblyName}/{assemblyName}_Definitions.cs");
        var domain = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_Domain.cs");
        var entity = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_Entity.cs");
        var matcher = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_Matcher.cs");
        var setter = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_Setter.cs");
        var executeSystem = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_ExecuteSystem.cs");
        var reactiveSystem = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_ReactiveSystem.cs");
        var generatorStats = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_GeneratorStats.cs");
        var componentAspect = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_IFirstComponentAspect.cs");
        var component = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_IFirstComponent.cs");

        var cdw = CDW.Configure()
            .AddGenerator(new Generator())
            .AddDependency("netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")
            .AddDependency("System.Runtime, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
            .AddDependency("System.Memory, Version=4.0.1.1, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")
            .AddDependency("SRGEnt.Core, Version=0.5.5.0, Culture=neutral")
            .AddSourceFile(source)
            .SetAssemblyName(assemblyName)
            .Build();
        cdw.Run();

        cdw.Diagnostics.Length.Should().Be(0);
        cdw.OutputCompilation.Should().NotBeNull();
        cdw.OutputCompilation?.SyntaxTrees.Count().Should().Be(10);
        cdw.OutputCompilation?.GetDiagnostics().Length.Should().Be(0);
        
        var sourceComparer = new SourceComparer(cdw);
        sourceComparer.CompareGeneratedToSourceString("SingleComponentTestDomain.Generated.cs",domain);
        sourceComparer.CompareGeneratedToSourceString("SingleComponentTestEntity.Generated.cs",entity);
        sourceComparer.CompareGeneratedToSourceString("SingleComponentTestAspectSetter.Generated.cs",setter);
        sourceComparer.CompareGeneratedToSourceString("SingleComponentTestMatcher.Generated.cs",matcher);
        sourceComparer.CompareGeneratedToSourceString("SingleComponentTestExecuteSystem.Generated.cs",executeSystem);
        sourceComparer.CompareGeneratedToSourceString("SingleComponentTestReactiveSystem.Generated.cs",reactiveSystem);
        sourceComparer.CompareGeneratedToSourceString("IFirstComponent.Generated.cs",component);
        sourceComparer.CompareGeneratedToSourceString("IFirstComponentAspect.Generated.cs",componentAspect);
        sourceComparer.CompareGeneratedToSourceString($"{assemblyName}.GeneratorStats.Generated.cs",generatorStats);
    }
    
    [Fact]
    public void PrimaryIndexComponentGeneratorTest()
    {
        const string assemblyName = "PrimaryIndexComponent";
        
        var source = File.ReadAllText($"{_inputDirectory}/{assemblyName}/{assemblyName}_Definitions.cs");
        var domain = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_Domain.cs");
        var entity = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_Entity.cs");
        var firstIndex = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_FirstComponentPrimaryIndex.cs");
        var secondIndex = File.ReadAllText($"{_outputDirectory}/{assemblyName}/{assemblyName}_SecondComponentPrimaryIndex.cs");

        var cdw = CDW.Configure()
            .AddGenerator(new Generator())
            .AddDependency("netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")
            .AddDependency("System.Runtime, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
            .AddDependency("System.Memory, Version=4.0.1.1, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")
            .AddDependency("SRGEnt.Core, Version=0.5.5.0, Culture=neutral")
            .AddSourceFile(source)
            .SetAssemblyName(assemblyName)
            .Build();
        cdw.Run();

        cdw.Diagnostics.Length.Should().Be(0);
        cdw.OutputCompilation.Should().NotBeNull();
        cdw.OutputCompilation?.SyntaxTrees.Count().Should().Be(14);
        cdw.OutputCompilation?.GetDiagnostics().Length.Should().Be(0);
        
        var sourceComparer = new SourceComparer(cdw);
        sourceComparer.CompareGeneratedToSourceString("PrimaryIndexComponentTestDomain.Generated.cs",domain);
        sourceComparer.CompareGeneratedToSourceString("PrimaryIndexComponentTestEntity.Generated.cs",entity);
        sourceComparer.CompareGeneratedToSourceString("IFirstComponentPrimaryIndex.Generated.cs",firstIndex);
        sourceComparer.CompareGeneratedToSourceString("ISecondComponentPrimaryIndex.Generated.cs",secondIndex);
    }

    private static void VerboseAssertDiagnosticsCount(CompilationDriverWrapper cdw)
    {
        var diagnostics = new StringBuilder();
        foreach (var diagnostic in cdw.OutputCompilation?.GetDiagnostics())
        {
            diagnostics.AppendLine(diagnostic.GetMessage());
        }

        Assert.True(cdw.OutputCompilation?.GetDiagnostics().Length == 0, diagnostics.ToString());
    }
}