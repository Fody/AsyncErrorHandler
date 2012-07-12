using System.Collections.Generic;
using Mono.Cecil;

public class AllTypesFinder
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
            if (typeDefinition.IsClass)
            {
                GetTypes(typeDefinition.NestedTypes);
                AllTypes.Add(typeDefinition);
            }
        }
    }
}