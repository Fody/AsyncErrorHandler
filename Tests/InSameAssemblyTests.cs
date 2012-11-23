using NUnit.Framework;

[TestFixture]
public class InSameAssemblyTests : TaskTests
{
	public InSameAssemblyTests():base(@"AssemblyToProcess\AssemblyToProcess.csproj")
	{
	}
}