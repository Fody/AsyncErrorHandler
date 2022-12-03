using System;
using System.Threading.Tasks;

public class Template
{
    public async Task Method()
    {
        await Task.Delay(1);
    }

    public async Task MethodWithThrow()
    {
        await Task.Delay(1);
        throw new Exception();
    }

    public async Task<int> MethodGeneric()
    {
        await Task.Delay(1);
        return 1;
    }

    public async Task<int> MethodWithThrowGeneric()
    {
        await Task.Delay(1);
        throw new Exception();
    }
}