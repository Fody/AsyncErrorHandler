using System;
using System.Diagnostics;

public static class AsyncErrorHandler
{
    public static void HandleExcption(Exception exception)
    {
        Debug.WriteLine(exception);
    }
}