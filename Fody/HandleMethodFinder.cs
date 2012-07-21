using System.Linq;
using Mono.Cecil;

public class HandleMethodFinder
{
    public ModuleDefinition ModuleDefinition;
    public MethodDefinition HandleMethod;


    public void Execute()
    {
        var errorHandler = ModuleDefinition.Types.FirstOrDefault(x => x.Name == "AsyncErrorHandler");
        if (errorHandler == null)
        {
            throw new WeavingException("Cound not find type 'AsyncErrorHandler'.");
        }
        HandleMethod = errorHandler.Methods.FirstOrDefault(x => x.Name == "HandleException");
        if (HandleMethod == null)
        {
            throw new WeavingException(string.Format("Could not find 'HandleException' method on '{0}'.", errorHandler.FullName));
        }
        if (!HandleMethod.IsPublic)
        {
            throw new WeavingException("Method 'AsyncErrorHandler.HandleException' is not public.");
        }
        if (!HandleMethod.IsStatic)
        {
            throw new WeavingException("Method 'AsyncErrorHandler.HandleException' is not static.");
        }
        if (HandleMethod.Parameters.Count != 1)
        {
            throw new WeavingException("Method 'AsyncErrorHandler.HandleException' must have only 1 parameter that is of type 'System.Exception'.");
        }
        var parameterDefinition = HandleMethod.Parameters.First();
        var parameterType = parameterDefinition.ParameterType;
        if (parameterType.FullName != "System.Exception")
        {
            throw new WeavingException("Method 'AsyncErrorHandler.HandleException' must have only 1 parameter that is of type 'System.Exception'.");
        }

    }

}