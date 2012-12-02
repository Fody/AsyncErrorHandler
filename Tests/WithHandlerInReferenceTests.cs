using NUnit.Framework;

[TestFixture]
public class WithHandlerInReferenceTests : TaskTests
{
	public WithHandlerInReferenceTests():base(@"AssemblyWithHandlerInReference\AssemblyWithHandlerInReference.csproj")
	{
	}
}