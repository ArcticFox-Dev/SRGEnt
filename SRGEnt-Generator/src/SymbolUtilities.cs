using Microsoft.CodeAnalysis;

namespace SRGEnt.Generator
{
    public static class SymbolUtilities
    {
        public static bool DoesSymbolExist(string metadataName, Compilation compilation)
        {
            var symbol = compilation.GetTypeByMetadataName(metadataName);
            return symbol != null;
        }
    }
}