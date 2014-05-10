using System;
using System.Diagnostics;

public static class AsyncErrorHandler
{
    public static void HandleException(Exception exception)
    {
        Debug.WriteLine(exception);
    }
}