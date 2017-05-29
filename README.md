Fody.AsyncErrorHandler
==================

![Icon](https://raw.github.com/Fody/AsyncErrorHandler/master/Icons/package_icon.png)

## What?

Fody.AsyncErrorHandler is a [Fody](https://github.com/Fody/Fody) extension for weaving exception handling code into applications which use async code.

[Introduction to Fody](https://github.com/Fody/Fody/wiki/SampleUsage)

[![NuGet Status](https://img.shields.io/gitter/room/fody/fody.svg?style=flat)](https://gitter.im/Fody/Fody)

## The nuget package  [![NuGet Status](http://img.shields.io/nuget/v/AsyncErrorHandler.Fody.svg?style=flat)](https://www.nuget.org/packages/AsyncErrorHandler.Fody/)

https://nuget.org/packages/AsyncErrorHandler.Fody/

    PM> Install-Package AsyncErrorHandler.Fody

## Why?

Because writing plumbing code is dumb and repetitive.

## How?

IL-weaving after the code is compiled, bro.

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
            catch (Exception exception)
            {
                AsyncErrorHandler.HandleException(exception);
            } 
        }
    }

And your application could provide its own implementation of the error handling module:


    public static class AsyncErrorHandler
    {
        public static void HandleException(Exception exception)
        {
            Debug.WriteLine(exception);
        }
    }

Which allows you to intercept the exceptions at runtime.

## What it really does

So the above example is actually a little misleading. It shows "in effect" what is inject. In reality the injected code is a little more complicated.

### What async actually produces

So given a method like this

    public async Task Method()
    {
        await Task.Delay(1);
    }
    
The compile will produce this 

    [AsyncStateMachine(typeof(<Method>d__0)), DebuggerStepThrough]
    public Task Method()
    {
        <Method>d__0 d__;
        d__.<>4__this = this;
        d__.<>t__builder = AsyncTaskMethodBuilder.Create();
        d__.<>1__state = -1;
        d__.<>t__builder.Start<<Method>d__0>(ref d__);
        return d__.<>t__builder.Task;
    }

So "Method" has become a stub that calls into a state machine.

The state machine will look like this

    [CompilerGenerated]
    struct <Method>d__0 : IAsyncStateMachine
    {
        // Fields
        public int <>1__state;
        public Target <>4__this;
        public AsyncTaskMethodBuilder <>t__builder;
        private object <>t__stack;
        private TaskAwaiter <>u__$awaiter1;

        // Methods
        private void MoveNext();
        [DebuggerHidden]
        private void SetStateMachine(IAsyncStateMachine param0);
    }


The method we care about is `MoveNext`. It will look something like this

	void MoveNext()
	{
	    try
	    {
	        TaskAwaiter awaiter;
	        bool flag = true;
	        switch (this.<>1__state)
	        {
	            case -3:
	                goto Label_009F;
	
	            case 0:
	                break;
	
	            default:
	                awaiter = Task.Delay(1).GetAwaiter();
	                if (awaiter.IsCompleted)
	                {
	                    goto Label_006F;
	                }
	                this.<>1__state = 0;
	                this.<>u__$awaiter1 = awaiter;
	                this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, Target.<Method>d__0>(ref awaiter, ref this);
	                flag = false;
	                return;
	        }
	        awaiter = this.<>u__$awaiter1;
	        this.<>u__$awaiter1 = new TaskAwaiter();
	        this.<>1__state = -1;
	    Label_006F:
	        awaiter.GetResult();
	        awaiter = new TaskAwaiter();
	    }
	    catch (Exception exception)
	    {
	        this.<>1__state = -2;
	        this.<>t__builder.SetException(exception);
	        return;
	    }
	Label_009F:
	    this.<>1__state = -2;
	    this.<>t__builder.SetResult();
	}


Most of that can be ignored. The important thing to note is that it is swallowing exceptions in a catch. And passing that exception to a `SetException` method.

So when AsyncErrorHandler does its weaving it searches for `SetException(exception);` and then modifies the catch to look like this.

    catch (Exception exception)
    {
        this.<>1__state = -2;
        AsyncErrorHandler.HandleException(exception);
        this.<>t__builder.SetException(exception);
        return;
    }
    
        
## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com)

