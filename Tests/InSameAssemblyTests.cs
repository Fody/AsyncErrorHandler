using System;
using System.Reflection;
using System.Threading.Tasks;
using Fody;
using TUnit.Assertions;
using TUnit.Core;

// the weaved assemblies expose a static exception field shared by the tests
[NotInParallel]
public class InSameAssemblyTests
{
    FieldInfo exceptionField;
    dynamic target;

    public InSameAssemblyTests()
    {
        var weaver = new ModuleWeaver();

        var testResult = weaver.ExecuteTestRun(
            "AssemblyToProcess.dll",
            assemblyName: "InSameAssembly",
            runPeVerify: false);
        target = testResult.GetInstance("Target");
        var errorHandler = testResult.Assembly.GetType("AsyncErrorHandler");
        exceptionField = errorHandler.GetField("Exception");
    }

    [Test]
    public async Task Method()
    {
        ClearException();
        await target.Method();
        await Assert.That(GetException()).IsNull();
    }

    [Test]
    public async Task MethodWithThrow()
    {
        ClearException();
        try
        {
            await target.MethodWithThrow();
        }
        catch
        {
        }

        await Assert.That(GetException()).IsNotNull();
    }

    [Test]
    public async Task MethodGeneric()
    {
        ClearException();
        await target.MethodGeneric();
        await Assert.That(GetException()).IsNull();
    }

    [Test]
    public async Task MethodWithThrowGeneric()
    {
        ClearException();
        try
        {
            await target.MethodWithThrowGeneric();
        }
        catch
        {
        }

        await Assert.That(GetException()).IsNotNull();
    }

    void ClearException()
    {
        exceptionField.SetValue(null, null);
    }

    Exception GetException()
    {
        return (Exception) exceptionField.GetValue(null);
    }
}