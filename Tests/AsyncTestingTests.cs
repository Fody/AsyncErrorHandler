using System;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

[TestFixture]
public class AsyncTestingTests
{
    public async Task AsyncMethod()
    {
        var webClient = new WebClient();
        var html = await webClient.DownloadStringTaskAsync(new Uri("http://www.google.com"));
        Debug.WriteLine(1);
        throw new Exception();
    }

    [Test]
    public void Test()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
         AsyncMethod();
        Debug.WriteLine(2);
        Thread.Sleep(3000);
    }

    void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        
        Debug.WriteLine(e.ExceptionObject);
    }
}