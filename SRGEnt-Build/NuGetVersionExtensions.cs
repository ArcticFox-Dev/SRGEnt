using NuGet.Versioning;

namespace a;

public enum NuGetVersionElement
{
Major,
Minor,
Patch
}

public static class NuGetVersionExtensions
{
    public static NuGetVersion Increment(this NuGetVersion version, NuGetVersionElement elementToIncrement)
    {
        NuGetVersion newVersion = default;
        switch (elementToIncrement)
        {
            case NuGetVersionElement.Major:
                newVersion = new NuGetVersion(version.Major + 1, 0, 0);
                break;
            case NuGetVersionElement.Minor:
                newVersion = new NuGetVersion(version.Major, version.Minor + 1, 0);
                break;
            case NuGetVersionElement.Patch:
                newVersion = new NuGetVersion(version.Major, version.Minor, version.Patch + 1);
                break;
        }
        return newVersion;
    }
}