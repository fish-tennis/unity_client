using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using UnityEditor;
using UnityEngine;


public class TestController : MonoBehaviour
{
    public bool run;
    public bool unLock;

    public bool test;

    // public TestView tview;
    private void Start()
    {
        // var assembly = AssemblyDefinition.ReadAssembly(AssemblyPostProcessor.mainPath);
        // var a = assembly.MainModule.Types;
    }

    private void OnValidate()
    {
        if (run)
        {
            run = false;
            TestMethod();
        }

        if (unLock)
        {
            unLock = false;
            EditorApplication.UnlockReloadAssemblies();
        }

        if (test)
        {
            test = false;
            TestCecil.Test();
        }
    }

    private void TestMethod()
    {
        //AssemblyPostProcessor.ProcessAssembly();
        
    }

    public static void TestDyna(string str)
    {
    }
}