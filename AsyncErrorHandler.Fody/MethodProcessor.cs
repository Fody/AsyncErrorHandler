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

            if (line.Operand is not MethodReference methodReference)
            {
                continue;
            }

            if (!IsSetExceptionMethod(methodReference))
            {
                continue;
            }

            var previous = instructions[index-1];
            instructions.Insert(index, Instruction.Create(OpCodes.Call, HandleMethodFinder.HandleMethod));
            index++;
            if (previous.Operand is not VariableDefinition variableDefinition)
            {
                throw new($"Expected VariableDefinition but got '{previous.Operand.GetType().Name}'.");
            }
            instructions.Insert(index, Instruction.Create(previous.OpCode, variableDefinition));
            index++;

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