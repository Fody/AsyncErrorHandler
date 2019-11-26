using System;
using System.Reflection;
using System.Threading.Tasks;
using Fody;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class WithHandlerInReferenceTests :
    VerifyBase
{
    FieldInfo exceptionField;
    dynamic target;

    public WithHandlerInReferenceTests(ITestOutputHelper output) :
        base(output)
    {
        var weavingTask = new ModuleWeaver();

        var testResult = weavingTask.ExecuteTestRun("AssemblyWithHandlerInReference.dll", runPeVerify: false);
        target = testResult.GetInstance("Target");
        var errorHandler = Type.GetType("AsyncErrorHandler, AssemblyToProcess");
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