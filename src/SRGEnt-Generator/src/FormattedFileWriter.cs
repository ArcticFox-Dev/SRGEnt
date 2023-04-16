using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SRGEnt.Generator
{
    public static class FormattedFileWriter
    {
        public static void WriteSourceFile(GeneratorExecutionContext context, string sourceText, string fileName)
        {
            var formattedSource = FormatSource(sourceText);
            context.AddSource($"{fileName}.Generated.cs", formattedSource);
        }

        public static string FormatSource(string sourceText)
        {
            var tree = CSharpSyntaxTree.ParseText(sourceText);
            var root = (CSharpSyntaxNode)tree.GetRoot();
            return root.NormalizeWhitespace().ToFullString();
        }

    }
}