using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

public class MethodProcessor
{
    public HandleMethodFinder HandleMethodFinder;


    public void Process(MethodDefinition method)
    {

        method.Body.SimplifyMacros();

        var instructions = method.Body.Instructions;
        for (var index = 0; index < instructions.Count; index++)
        {
            var line = instructions[index];
            if (line.OpCode != OpCodes.Call)
            {
                continue;
            }
            var methodReference = line.Operand as MethodReference;
            if (methodReference == null)
            {
                continue;
            }

            if (IsSetExceptionMethod(methodReference))
            {
                var previous = instructions[index-1];
                instructions.Insert(index, Instruction.Create(OpCodes.Call, HandleMethodFinder.HandleMethod));
                index++;
                instructions.Insert(index, Instruction.Create(previous.OpCode, (VariableDefinition)previous.Operand));
                index++;
            }

        }
        method.Body.OptimizeMacros();
    }

    public static bool IsSetExceptionMethod(MethodReference methodReference)
    {
        return 
            methodReference.Name == "SetException" && 
            methodReference.DeclaringType.FullName.StartsWith("System.Runtime.CompilerServices.AsyncTaskMethodBuilder");
    }
}