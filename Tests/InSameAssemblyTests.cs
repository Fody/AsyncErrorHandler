using System;
using System.Reflection;
using System.Threading.Tasks;
using Fody;
using Xunit;

public class InSameAssemblyTests
{
    FieldInfo exceptionField;
    dynamic target;

    public InSameAssemblyTests()
    {
        var weaver = new ModuleWeaver();

        var testResult = weaver.ExecuteTestRun("AssemblyToProcess.dll",
            assemblyName: "InSameAssembly",
            runPeVerify: false);
        target = testResult.GetInstance("Target");
        var errorHandler = testResult.Assembly.GetType("AsyncErrorHandler");
        exceptionField = errorHandler.GetField("Exception");
    }

    [Fact]
    public async Task Method()
    {
        ClearException();
        await target.Method();
        Assert.Null(GetException());
    }

    [Fact]
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

        Assert.NotNull(GetException());
    }

    [Fact]
    public async Task MethodGeneric()
    {
        ClearException();
        await target.MethodGeneric();
        Assert.Null(GetException());
    }

    [Fact]
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

        Assert.NotNull(GetException());
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