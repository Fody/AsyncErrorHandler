using System;
using System.IO;
using System.Reflection;
using Mono.Cecil;

public class MockAssemblyResolver : IAssemblyResolver
{
    public AssemblyDefinition Resolve(AssemblyNameReference name)
    {
        var fileName = Path.Combine(Directory, name.Name) + ".dll";
        if (File.Exists(fileName))
        {
            return AssemblyDefinition.ReadAssembly(fileName);
        }
        var codeBase = Assembly.Load(name.FullName).CodeBase.Replace("file:///", "");
        return AssemblyDefinition.ReadAssembly(codeBase);
    }

    public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
    {
        var codeBase = Assembly.Load(name.FullName).CodeBase.Replace("file:///","");

        return AssemblyDefinition.ReadAssembly(codeBase);
    }

    public string Directory;
    public void Dispose()
    {
    }
}