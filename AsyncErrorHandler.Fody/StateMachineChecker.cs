using System.Linq;
using Mono.Cecil;

public static class StateMachineChecker
{
    public static bool IsStateMachine(this TypeDefinition typeDefinition)
    {
        return typeDefinition.IsIAsyncStateMachine() &&
            typeDefinition.IsCompilerGenerated();
    }

    public static bool IsCompilerGenerated(this TypeDefinition typeDefinition)
    {
        return typeDefinition.CustomAttributes.Any(_ => _.Constructor.DeclaringType.Name == "CompilerGeneratedAttribute");
    }

    public static bool IsIAsyncStateMachine(this TypeDefinition typeDefinition)
    {
        return typeDefinition.Interfaces.Any(_ => _.InterfaceType.Name =="IAsyncStateMachine");
    }
}