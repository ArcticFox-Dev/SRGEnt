using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SRGEnt.Generator
{
    public static class FormattedFileWriter
    {
        public static void WriteSourceFile(GeneratorExecutionContext context, string sourceText, string fileName)
        {
            var tree = CSharpSyntaxTree.ParseText(sourceText);
            var root = (CSharpSyntaxNode) tree.GetRoot();
            var formattedSource = root.NormalizeWhitespace().ToFullString();
            context.AddSource($"{fileName}.Generated.cs", formattedSource);
        }
    }
}