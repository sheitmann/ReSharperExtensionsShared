﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Infrastructure;
using JetBrains.Application;
using JetBrains.Threading;
using JetBrains.Util;
using NUnit.Framework;

[assembly: TestDataPathBase(@"TestData")]

/// <summary>
/// Test environment. Must be in the global namespace.
/// </summary>
[SetUpFixture]
// ReSharper disable once CheckNamespace
public class TestEnvironmentSetUpFixture : ReSharperTestEnvironmentAssembly
{
    /// <summary>
    /// Gets the assemblies to load into test environment.
    /// Should include all assemblies which contain components.
    /// </summary>
    private static IEnumerable<Assembly> GetAssembliesToLoad()
    {
        // Test assembly
        yield return Assembly.GetExecutingAssembly();

        yield return typeof(DebugUtilities).Assembly;
    }

    public override void SetUp()
    {
        base.SetUp();
        ReentrancyGuard.Current.Execute(
            "LoadAssemblies",
            () => Shell.Instance.GetComponent<AssemblyManager>().LoadAssemblies(GetType().Name, GetAssembliesToLoad()));
    }

    public override void TearDown()
    {
        ReentrancyGuard.Current.Execute(
            "UnloadAssemblies",
            () => Shell.Instance.GetComponent<AssemblyManager>().UnloadAssemblies(GetType().Name, GetAssembliesToLoad()));
        base.TearDown();
    }
}