using System;
using System.Linq;
using Mono.Cecil;

public class ModuleWeaver
{
    public Action<string> LogInfo { get; set; }
    public Action<string> LogWarning { get; set; }
	public ModuleDefinition ModuleDefinition { get; set; }
	public IAssemblyResolver AssemblyResolver { get; set; }

    public ModuleWeaver()
    {
        LogInfo = s => { };
        LogWarning = s => { };
    }

    public void Execute()
    {
        var initializeMethodFinder = new HandleMethodFinder
            {
                ModuleDefinition = ModuleDefinition,
				AssemblyResolver = AssemblyResolver
            };
        initializeMethodFinder.Execute();

        var stateMachineFinder = new StateMachineTypesFinder
            {
                ModuleDefinition = ModuleDefinition,
            };
        stateMachineFinder.Execute();

        var methodProcessor = new MethodProcessor
            {
                HandleMethodFinder = initializeMethodFinder,
            };

        foreach (var stateMachine in stateMachineFinder.AllTypes)
        {
            var moveNext = stateMachine.Methods.First(x => x.Name == "MoveNext");
            methodProcessor.Process(moveNext);
        }

    }
}