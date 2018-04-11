﻿using System;
using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;

public class ModuleWeaver : BaseModuleWeaver
{
    public IAssemblyResolver AssemblyResolver { get; set; }

    public ModuleWeaver()
    {
        LogInfo = s => { };
        LogWarning = s => { };
    }

    public override void Execute()
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
            try
            {
                var moveNext = stateMachine.Methods.First(x => x.Name == "MoveNext");
                methodProcessor.Process(moveNext);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to process '{stateMachine.FullName}'.", exception);
            }
        }

    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        throw new NotImplementedException();
    }
}