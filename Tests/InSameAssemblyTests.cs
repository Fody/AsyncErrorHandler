using System;
using System.Reflection;
using System.Threading;
using Fody;
using Xunit;

public class InSameAssemblyTests
{
    FieldInfo exceptionField;
    dynamic target;

    public InSameAssemblyTests()
    {
        var weavingTask = new ModuleWeaver();

        var testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll");
        target = testResult.GetInstance("Target");
        var errorHandler = testResult.Assembly.GetType("AsyncErrorHandler");
        exceptionField = errorHandler.GetField("Exception");
    }

    [Fact]
    public void Method()
    {
        ClearException();
        target.Method();
        Thread.Sleep(100);
        Assert.Null(GetException());
    }

    [Fact]
    public void MethodWithThrow()
    {
        ClearException();
        target.MethodWithThrow();
        Thread.Sleep(100);
        Assert.NotNull(GetException());
    }

    [Fact]
    public void MethodGeneric()
    {
        ClearException();
        target.MethodGeneric();
        Thread.Sleep(100);
        Assert.Null(GetException());
    }

    [Fact]
    public void MethodWithThrowGeneric()
    {
        ClearException();
        target.MethodWithThrowGeneric();
        Thread.Sleep(100);
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