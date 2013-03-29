using System;

public class AsyncErrorHandling
{
    public static Exception Exception;
    public static void HandleException(Exception exception)
    {
        Exception = exception;
    }
}