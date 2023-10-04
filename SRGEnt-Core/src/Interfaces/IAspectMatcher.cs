namespace SRGEnt.Interfaces
{
    public interface IAspectMatcher<out TMatcher> where TMatcher : IAspectMatcher<TMatcher>
    {
        TMatcher Requires { get; }
        TMatcher CannotHave { get; }
        TMatcher ShouldHaveAtLeastOneOf { get; }
        void RecalculateHash();
    }
}