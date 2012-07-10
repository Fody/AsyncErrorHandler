Fody.ErrorHandling
==================

## What?

Fody.ErrorHandling is an extension for weaving exception handling code into applications which use async code.

## Why?

Because writing plumbing code is dumb and repetitive.

## How?

IL-weaving after the code has compiled, bro.

For example, imagine you've got this code to serialize an object to the filesystem:

    public class DataStorage
    {
        public async Task WriteFile(string key, object value)
        {
            var jsonValue = JsonConvert.SerializeObject(value);
            using (var file = await folder.OpenStreamForWriteAsync(key, CreationCollisionOption.ReplaceExisting))
            using (var stream = new StreamWriter(file))
                await stream.WriteAsync(jsonValue);
        }
    }

After the code builds, the weaver could scan your assembly looking for code which behaves a certain way, and rewrite it to include the necessary handling code:

    public class DataStorage
    {
        public async Task WriteFile(string key, object value)
        {
            try 
            {
                var jsonValue = JsonConvert.SerializeObject(value);
                using (var file = await folder.OpenStreamForWriteAsync(key, CreationCollisionOption.ReplaceExisting))
                using (var stream = new StreamWriter(file))
                    await stream.WriteAsync(jsonValue);
            }
            catch (Exception ex)
            {
                Fody.ErrorHandling.HandleException(ex);
            } 
        }
    }

And your application could provide its own implementation of the error handling module:

    public class DefaultExceptionHandler : IExceptionHandler
    {
        public void HandleException(ExceptionEventArgs args)
        {
           // TODO: whatever you want to log about the exception
           args.Handled = true;
        }
    }

Which allows you to intercept the exceptions at runtime.

