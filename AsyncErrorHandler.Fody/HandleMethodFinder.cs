using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;

public class HandleMethodFinder
{
    public ModuleDefinition ModuleDefinition;
    public MethodReference HandleMethod;
    public IAssemblyResolver AssemblyResolver;

    public void Execute()
    {
        var errorHandler = GetTypeDefinition();
        var handleMethod = errorHandler.Methods.FirstOrDefault(_ => _.Name == "HandleException");
        if (handleMethod == null)
        {
            throw new WeavingException($"Could not find 'HandleException' method on '{errorHandler.FullName}'.");
        }
        if (!handleMethod.IsPublic)
        {
            throw new WeavingException("Method 'AsyncErrorHandler.HandleException' is not public.");
        }
        if (!handleMethod.IsStatic)
        {
            throw new WeavingException("Method 'AsyncErrorHandler.HandleException' is not static.");
        }
        if (handleMethod.Parameters.Count != 1)
        {
            throw new WeavingException("Method 'AsyncErrorHandler.HandleException' must have only 1 parameter that is of type 'System.Exception'.");
        }
        var parameterDefinition = handleMethod.Parameters.First();
        var parameterType = parameterDefinition.ParameterType;
        if (parameterType.FullName != "System.Exception")
        {
            throw new WeavingException("Method 'AsyncErrorHandler.HandleException' must have only 1 parameter that is of type 'System.Exception'.");
        }
        HandleMethod = ModuleDefinition.ImportReference(handleMethod);
    }

    TypeDefinition GetTypeDefinition()
    {
        foreach (var module in GetAllModulesToSearch())
        {
            var errorHandler = module.GetTypes().FirstOrDefault(_ => _.Name == "AsyncErrorHandler");
            if (errorHandler != null)
            {
                return errorHandler;
            }
        }
        var error =
            """
            Could not find type 'AsyncErrorHandler'. Expected to find a class with the following signature.
            public static class AsyncErrorHandler
            {
                public static void HandleException(Exception exception)
                {
                    Debug.WriteLine("Exception occurred: " + exception.Message);
                }
            }
            """;

        throw new WeavingException(error);
    }

    IEnumerable<ModuleDefinition> GetAllModulesToSearch()
    {
        yield return ModuleDefinition;

        foreach (var reference in ModuleDefinition.AssemblyReferences.Where(x => !IsMicrosoftAssembly(x)))
        {
            yield return ModuleDefinition.AssemblyResolver.Resolve(reference).MainModule;
        }
    }

    static bool IsMicrosoftAssembly(AssemblyNameReference reference)
    {
        return reference.FullName.EndsWith("b77a5c561934e089");
    }
}