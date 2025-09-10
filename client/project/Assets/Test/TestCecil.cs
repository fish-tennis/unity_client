using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEditor;
using UnityEngine;

public static class TestCecil
{
    public static void Test()
    {
        //读取指定程序集
        var assembly = AssemblyDefinition.ReadAssembly("./Library/ScriptAssemblies/MVVMFrame.dll");
        //锁定程序集编译
        EditorApplication.LockReloadAssemblies();
        try
        {
            foreach (var typeDefinition in assembly.MainModule.Types)
            {
                if (typeDefinition.FullName == "TestView")
                {
                    //获取已有的方法
                    var tmethod = typeDefinition.Methods.FirstOrDefault(m => m.Name == "Test");
                    //根据特性获取方法
                    // var tmethod = typeDefinition.Methods.FirstOrDefault(m =>
                    //     m.CustomAttributes.FirstOrDefault(a => a.AttributeType.Name == "自定义标签") != null
                    // );
                    if (tmethod != null)
                    {
                        //获取方法的IL(微软的中间语言)处理器,IL主要包含一些元数据和中间语言指令
                        var il = tmethod.Body.GetILProcessor();
                        //获取tmethod的当前首行位置
                        var point = tmethod.Body.Instructions.First();
                        // 插入point之前
                        il.InsertBefore(point, il.Create(OpCodes.Nop));
                        // //推送对元数据中存储的字符串的新对象引用
                        il.InsertBefore(point,il.Create(OpCodes.Ldstr, "注入打印"));
                        //获取引用:Debug.LogError
                        var logMethodInfo = typeof(Debug).GetMethod("LogError", new Type[] {typeof(object)});
                        var logRef = assembly.MainModule.ImportReference(logMethodInfo);
                        //调用Debug.LogError方法
                        il.InsertBefore(point, il.Create(OpCodes.Call, logRef));
                        //重新计算方法的Size
                        ComputeOffSets(tmethod.Body);
                    }
                }
            }

            //重新写入程序集
            assembly.Write("./Library/ScriptAssemblies/MVVMFrame.dll");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        //解锁程序集编译
        EditorApplication.UnlockReloadAssemblies();
        Debug.LogError("注入完成");
    }

    //重新计算方法Size
    private static void ComputeOffSets(MethodBody body)
    {
        var offset = 0;
        foreach (var instruction in body.Instructions)
        {
            instruction.Offset = offset;
            offset += instruction.GetSize();
        }

        body.MaxStackSize = 8;
    }
}