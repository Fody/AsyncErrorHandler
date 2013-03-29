using System;

public static class AsyncErrorHandler
{
    public static Exception Exception;
    public static void HandleException(Exception exception)
    {
        Exception = exception;
    }
}