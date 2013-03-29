using System.Collections.Generic;
using Mono.Cecil;

public class StateMachineTypesFinder
{

    public List<TypeDefinition> AllTypes;
    public ModuleDefinition ModuleDefinition;

    public void Execute()
    {
        AllTypes = new List<TypeDefinition>();
        GetTypes(ModuleDefinition.Types);
    }

    void GetTypes(IEnumerable<TypeDefinition> typeDefinitions)
    {
        foreach (var typeDefinition in typeDefinitions)
        {
            GetTypes(typeDefinition.NestedTypes);
            if (typeDefinition.IsStateMachine())
            {
                AllTypes.Add(typeDefinition);
            }
        }
    }
}