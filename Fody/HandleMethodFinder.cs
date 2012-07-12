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
            throw new WeavingException(string.Format("Could not find 'Initialize' method on '{0}'.", errorHandler.FullName));
        }
        if (!HandleMethod.IsPublic)
        {
            throw new WeavingException(string.Format("Method '{0}' is not public.", HandleMethod.FullName));
        }
        if (!HandleMethod.IsStatic)
        {
            throw new WeavingException(string.Format("Method '{0}' is not static.", HandleMethod.FullName));
        }
        if (HandleMethod.Parameters.Count != 1)
        {
            throw new WeavingException(string.Format("Method '{0}' must have 1 parameter of type 'System.Exception'.", HandleMethod.FullName));
        }
        var parameterDefinition = HandleMethod.Parameters.First();
        var parameterType = parameterDefinition.ParameterType;
        if (parameterType.FullName != "System.Exception")
        {
            throw new WeavingException(string.Format("Method '{0}' must have 1 parameter of type 'System.Exception'.", HandleMethod.FullName));
        }

    }

}