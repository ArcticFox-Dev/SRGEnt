using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SRGEnt.Generator.Tests.Utilities;

public class CompilationDriverWrapper
{
    public Compilation InputCompilation
    {
        get;
    }
    public GeneratorDriver Driver
    {
        get;
        private set;
    }
    public Compilation? OutputCompilation { get; private set; }
    public ImmutableArray<Diagnostic> Diagnostics { get; private set; }

    public static Builder Configure()
    {
        return new Builder();
    }

    private CompilationDriverWrapper(Compilation inputCompilation, GeneratorDriver driver)
    {
        InputCompilation = inputCompilation;
        Driver = driver;
    }

    public void Run()
    {
        Driver = Driver.RunGeneratorsAndUpdateCompilation(InputCompilation, out var outputCompilation, out var diagnostics);
        OutputCompilation = outputCompilation;
        Diagnostics = diagnostics;
    }

    public string GetGeneratedSource(string fileName)
    {
        var generatedSources = Driver.GetRunResult().Results.First().GeneratedSources;
        GeneratedSourceResult targetFile;

        try
        {
            targetFile = Driver.GetRunResult().Results.First().GeneratedSources.First(gs => string.CompareOrdinal(gs.HintName,fileName) == 0);
        }
        catch (Exception e)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine($"Failed to find a generated file with the name:\n*** {fileName} ***\n");
            sb.AppendLine("Available Generated Files:");
            foreach (var generatedSource in generatedSources)
            {
                sb.AppendLine(generatedSource.HintName.ToString());
            }
            Assert.Fail(sb.ToString());
        }

        return targetFile.SourceText.ToString();
    }
    
    public class Builder
    {
        private string _assemblyName = "CompilationDataWrapper"; 
        private readonly List<ISourceGenerator> _generators = new List<ISourceGenerator>();
        private readonly List<string> _sources = new List<string>();
        private readonly List<string> _dependencies = new List<string>();

        public Builder AddGenerator(ISourceGenerator generator)
        {
            _generators.Add(generator);
            return this;
        }
        public Builder AddSourceFile(string source)
        {
            _sources.Add(source);
            return this;
        }

        public Builder AddDependency(string dependency)
        {
            _dependencies.Add(dependency);
            return this;
        }

        public Builder SetAssemblyName(string assemblyName)
        {
            _assemblyName = assemblyName;
            return this;
        }

        public CompilationDriverWrapper Build()
        {
            return new CompilationDriverWrapper(inputCompilation:CreateCompilation(),CSharpGeneratorDriver.Create(_generators.ToArray()));
        }
        
        private Compilation CreateCompilation()
        {
            var syntaxTrees = new List<SyntaxTree>();
            var references = new List<MetadataReference>();
            
            foreach (var source in _sources)
            {
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(source));
            }
            
            references.Add(MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(Console).Assembly.Location));

            foreach (var dependency in _dependencies)
            {
                references.Add(MetadataReference.CreateFromFile(Assembly.Load(dependency).Location));
            }
            
            var compilation = CSharpCompilation.Create(_assemblyName,                
                syntaxTrees.ToArray(),
                references.ToArray(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            return compilation;
        }
    }
}