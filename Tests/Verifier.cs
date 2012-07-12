﻿using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

public static class Verifier
{
    public static void Verify(string assemblyPath2)
    {
        var exePath = Environment.ExpandEnvironmentVariables(@"%programfiles(x86)%\Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\PEVerify.exe");

        if (!File.Exists(exePath))
        {
            exePath = Environment.ExpandEnvironmentVariables(@"%programfiles(x86)%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\PEVerify.exe");
        }
        var process = Process.Start(new ProcessStartInfo(exePath, "\"" + assemblyPath2 + "\"")
                                        {
                                            RedirectStandardOutput = true,
                                            UseShellExecute = false,
                                            CreateNoWindow = true
                                        });

        process.WaitForExit(10000);
        var readToEnd = process.StandardOutput.ReadToEnd().Trim();
        Assert.IsTrue(readToEnd.Contains(string.Format("All Classes and Methods in {0} Verified.", assemblyPath2)), readToEnd);
    }
}