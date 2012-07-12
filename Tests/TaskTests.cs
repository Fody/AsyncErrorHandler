using System;
using System.Reflection;
using System.Threading;
using NUnit.Framework;

[TestFixture]
public class TaskTests
{
    string projectPath;
    Assembly assembly;
    FieldInfo exceptionField;

    public  TaskTests()
    {
        projectPath = @"AssemblyToProcess\AssemblyToProcess.csproj";
#if (!DEBUG)
        projectPath = projectPath.Replace("Debug", "Release");
#endif
    }

    [TestFixtureSetUp]
    public void Setup()
    {
        var weaverHelper = new WeaverHelper(projectPath);
        assembly = weaverHelper.Assembly;
        var errorHandler = assembly.GetType("AsyncErrorHandler");
        exceptionField = errorHandler.GetField("Exception");
    }


    [Test]
    public void Method()
    {
        ClearException();
        var instance = assembly.GetInstance("Target");
        instance.Method();
        Thread.Sleep(100);
        Assert.IsNull(GetException());
    }
    [Test]
    public void MethodWithThrow()
    {
        ClearException();
        var instance = assembly.GetInstance("Target");
        instance.MethodWithThrow();
        Thread.Sleep(100);
        Assert.IsNotNull(GetException());
    }
    [Test]
    public void MethodGeneric()
    {
        ClearException();
        var instance = assembly.GetInstance("Target");
        instance.MethodGeneric();
        Thread.Sleep(100);
        Assert.IsNull(GetException());
    }
    [Test]
    public void MethodWithThrowGeneric()
    {
        ClearException();
        var instance = assembly.GetInstance("Target");
        instance.MethodWithThrowGeneric();
        Thread.Sleep(100);
        Assert.IsNotNull(GetException());
    }

    private void ClearException()
    {
         exceptionField.SetValue(null, null);
    }

    Exception GetException()
    {
        return (Exception) exceptionField.GetValue(null);
    }


#if(DEBUG)
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(assembly.CodeBase.Remove(0, 8));
    }
#endif

}