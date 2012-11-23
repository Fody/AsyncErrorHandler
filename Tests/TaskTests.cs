using System;
using System.IO;
using System.Reflection;
using System.Threading;
using NUnit.Framework;

public class TaskTests
{
    string projectPath;
    Assembly assembly;
    FieldInfo exceptionField;

	public TaskTests(string projectPath)
    {
		this.projectPath = projectPath;
#if (!DEBUG)
        this.projectPath = this.projectPath.Replace("Debug", "Release");
#endif
		var weaverHelper = new WeaverHelper(this.projectPath);
        assembly = weaverHelper.Assembly;

		var directoryName = Path.GetDirectoryName(assembly.Location);
	    var combine = Path.Combine(directoryName, "AssemblyToProcess.dll");
	    var loadFile = Assembly.LoadFrom(combine);
	    var errorHandler = loadFile.GetType("AsyncErrorHandler");
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