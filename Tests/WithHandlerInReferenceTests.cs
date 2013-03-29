using System;
using System.IO;
using System.Reflection;
using System.Threading;
using NUnit.Framework;

[TestFixture]
public class WithHandlerInReferenceTests 
{
    string beforeAssemblyPath;
    Assembly assembly;
    FieldInfo exceptionField;
    string afterAssemblyPath;

    public WithHandlerInReferenceTests()
    {
        beforeAssemblyPath = Path.GetFullPath(@"..\..\..\AssemblyWithHandlerInReference\bin\debug\AssemblyWithHandlerInReference.dll");
#if (!DEBUG)
        beforeAssemblyPath = this.beforeAssemblyPath.Replace("debug", "Release");
#endif
        afterAssemblyPath = WeaverHelper.Weave(beforeAssemblyPath);


        assembly = Assembly.LoadFrom(afterAssemblyPath);
         
		var directoryName = Path.GetDirectoryName(assembly.Location);
	    var combine = Path.Combine(directoryName, "AssemblyToProcess.dll");
	    var refFile = Assembly.LoadFrom(combine);
        var errorHandler = refFile.GetType("AsyncErrorHandler");
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

    void ClearException()
    {
        exceptionField.SetValue(null, null);
    }

    Exception GetException()
    {
        return (Exception) exceptionField.GetValue(null);
    }

    [Test]
    public void PeVerify()
    {
        Verifier.Verify(beforeAssemblyPath,beforeAssemblyPath);
    }
}