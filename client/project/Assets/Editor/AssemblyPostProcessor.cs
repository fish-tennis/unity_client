using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using MethodBody = Mono.Cecil.Cil.MethodBody;


public static class AssemblyPostProcessor
{
    public static string mainPath = "./Library/ScriptAssemblies/MVVMFrame.dll";

    [MenuItem("Tools/注入绑定")]
    [PostProcessBuild]
    public static void ProcessAssembly()
    {
        if (EditorApplication.isCompiling)
            return;
        var assembly = AssemblyDefinition.ReadAssembly(mainPath);
        EditorApplication.LockReloadAssemblies();
        try
        {
            var mainModule = assembly.MainModule;
            foreach (var typeDefinition in mainModule.Types)
            {
                var elementType = typeDefinition.BaseType?.GetElementType();
                if (elementType?.FullName == "ViewBase`2")
                {
                    var vmTypeDef =
                        (TypeDefinition) ((GenericInstanceType) typeDefinition.BaseType).GenericArguments[0];
                    var modelTypeDef =
                        (TypeDefinition) ((GenericInstanceType) typeDefinition.BaseType).GenericArguments[1];
                    TypeDefinition baseTypeDef = typeDefinition.BaseType.Resolve();
                    TypeDefinition vmbaseTypeDef = vmTypeDef.BaseType.Resolve();
                    InjectViewMethod(typeDefinition, vmTypeDef, baseTypeDef, "_viewModel");
                    InjectViewMethod(vmTypeDef, modelTypeDef, vmbaseTypeDef, "_model");
                }
            }

            assembly.Write(mainPath);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        EditorApplication.UnlockReloadAssemblies();
    }


    private static void InjectViewMethod(TypeDefinition typeDefinition, TypeDefinition vmTypeDef,
        TypeDefinition baseTypeDef, string vmName)
    {
        var bindMethod = typeDefinition.Methods.FirstOrDefault(m => m.Name == "BindProperty");
        var unbindMethod = typeDefinition.Methods.FirstOrDefault(m => m.Name == "UnBindProperty");
        var needBind = bindMethod == null;
        var needUnBind = unbindMethod == null;
        if (!needBind && !needUnBind)
            return;

        //创建要注入的绑定方法
        if (needBind)
        {
            bindMethod = new MethodDefinition("BindProperty",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeDefinition.Module.TypeSystem.Void);
            // bindMethod.Body.InitLocals = true;
        }


        //创建要注入的解绑方法
        if (needUnBind)
        {
            unbindMethod = new MethodDefinition("UnBindProperty",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeDefinition.Module.TypeSystem.Void);
            // unbindMethod.Body.InitLocals = true;
        }

        //遍历viewModel需要绑定的属性

        foreach (var field in vmTypeDef.Fields)
        {
            if (field.FieldType.Name == "BindableProperty`1")
            {
                var fieldName = field.Name;
                var methodName = "OnChanged_" + fieldName;
                var fieldTypeInstance = (GenericInstanceType) field.FieldType;
                var ArgumentType = fieldTypeInstance.GenericArguments[0];
                var OnChanged_Def = typeDefinition.Methods.FirstOrDefault(m => m.Name == methodName);

                if (OnChanged_Def != null && OnChanged_Def.Parameters.Count == 2 &&
                    OnChanged_Def.Parameters[0].ParameterType == ArgumentType &&
                    OnChanged_Def.Parameters[1].ParameterType == ArgumentType)
                {
                    if (needBind)
                        InjectBindMethod(bindMethod, typeDefinition, field, OnChanged_Def, baseTypeDef, vmName);
                    if (needUnBind)
                        InjectUnBindMethod(unbindMethod, typeDefinition, field, OnChanged_Def, baseTypeDef, vmName);
                }
            }
        }

        if (needBind)
        {
            var bindIL = bindMethod.Body.GetILProcessor();
            bindIL.Append(bindIL.Create(OpCodes.Ret));
            ComputeOffSets(bindMethod.Body);
            typeDefinition.Methods.Add(bindMethod);
        }

        if (needUnBind)
        {
            var unbindIL = unbindMethod.Body.GetILProcessor();
            unbindIL.Append(unbindIL.Create(OpCodes.Ret));
            ComputeOffSets(unbindMethod.Body);
            typeDefinition.Methods.Add(unbindMethod);
        }
    }

    private static void InjectBindMethod(MethodDefinition method, TypeDefinition typeDefinition,
        FieldDefinition field, MethodDefinition OnChanged_Def, TypeDefinition baseTypeDef, string vmName)
    {
        var il = method.Body.GetILProcessor();
        var fieldTypeInstance = (GenericInstanceType) field.FieldType;
        var ArgumentType = fieldTypeInstance.GenericArguments[0];
        var basevmDef = baseTypeDef.Fields.FirstOrDefault(f => f.Name == vmName);
        var basevmRef = typeDefinition.Module.ImportReference(basevmDef);
        var baseTypeInstance = typeDefinition.BaseType as GenericInstanceType;
        basevmRef = baseTypeInstance == null
            ? basevmRef
            : basevmRef.MakeGeneric(baseTypeInstance.GenericArguments.ToArray());
        var fieldRef = typeDefinition.Module.ImportReference(field); //BindableProperty<T> ViewModel.XX
        var fieldFDef = field.FieldType.Resolve();
        var ovcDef = fieldFDef.Fields.FirstOrDefault(f =>
            f.Name == "OnValueChanged"); //ViewModel.XX.OnValueChanged
        var ovcRef = typeDefinition.Module.ImportReference(ovcDef);
        ovcRef = ovcRef.MakeGeneric(fieldTypeInstance.GenericArguments.ToArray());

        var OnChanged_Ref = typeDefinition.Module.ImportReference(OnChanged_Def);
        var argType = Type.GetType($"{ArgumentType.FullName},{ArgumentType.Scope}");
        var actiontype = typeof(Action<,>).MakeGenericType(argType, argType);
        var actionctor = actiontype.GetConstructor(new Type[]
            {typeof(object), typeof(IntPtr)});
        var actionRef = typeDefinition.Module.ImportReference(actionctor);
        var actionTypeRef = typeDefinition.Module.ImportReference(actiontype);
        var cbRef = typeDefinition.Module.ImportReference(typeof(Delegate).GetMethod("Combine",
            new Type[] {typeof(Delegate), typeof(Delegate)})); //Delegate.Combine

        il.Append(il.Create(OpCodes.Ldarg_0));
        il.Append(il.Create(OpCodes.Ldfld, basevmRef));
        il.Append(il.Create(OpCodes.Ldfld, fieldRef));
        il.Append(il.Create(OpCodes.Dup));
        il.Append(il.Create(OpCodes.Ldfld, ovcRef));
        il.Append(il.Create(OpCodes.Ldarg_0));
        il.Append(il.Create(OpCodes.Ldftn, OnChanged_Ref));
        il.Append(il.Create(OpCodes.Newobj, actionRef));
        il.Append(il.Create(OpCodes.Call, cbRef));
        il.Append(il.Create(OpCodes.Castclass, actionTypeRef));
        il.Append(il.Create(OpCodes.Stfld, ovcRef));
    }

    private static void InjectUnBindMethod(MethodDefinition method, TypeDefinition typeDefinition,
        FieldDefinition field, MethodDefinition OnChanged_Def, TypeDefinition baseTypeDef, string vmName)
    {
        var il = method.Body.GetILProcessor();
        var fieldTypeInstance = (GenericInstanceType) field.FieldType;
        var ArgumentType = fieldTypeInstance.GenericArguments[0];
        var basevmDef = baseTypeDef.Fields.FirstOrDefault(f => f.Name == vmName);
        var basevmRef = typeDefinition.Module.ImportReference(basevmDef);
        var baseTypeInstance = typeDefinition.BaseType as GenericInstanceType;
        basevmRef = baseTypeInstance == null
            ? basevmRef
            : basevmRef.MakeGeneric(baseTypeInstance.GenericArguments.ToArray());

        var fieldRef = typeDefinition.Module.ImportReference(field); //BindableProperty<T> ViewModel.XX
        var fieldFDef = field.FieldType.Resolve();
        var ovcDef = fieldFDef.Fields.FirstOrDefault(f =>
            f.Name == "OnValueChanged"); //ViewModel.XX.OnValueChanged
        var ovcRef = typeDefinition.Module.ImportReference(ovcDef);
        ovcRef = ovcRef.MakeGeneric(fieldTypeInstance.GenericArguments.ToArray());

        var OnChanged_Ref = typeDefinition.Module.ImportReference(OnChanged_Def);
        var argType = Type.GetType($"{ArgumentType.FullName},{ArgumentType.Scope}");
        var actiontype = typeof(Action<,>).MakeGenericType(argType, argType);
        var actionctor = actiontype.GetConstructor(new Type[] {typeof(object), typeof(IntPtr)});
        var actionRef = typeDefinition.Module.ImportReference(actionctor);
        var actionTypeRef = typeDefinition.Module.ImportReference(actiontype);
        var RemoveRef =
            typeDefinition.Module.ImportReference(typeof(Delegate).GetMethod("Remove",
                new Type[] {typeof(Delegate), typeof(Delegate)})); //Delegate.Remove

        il.Append(il.Create(OpCodes.Ldarg_0));
        il.Append(il.Create(OpCodes.Ldfld, basevmRef));
        il.Append(il.Create(OpCodes.Ldfld, fieldRef));
        il.Append(il.Create(OpCodes.Dup));
        il.Append(il.Create(OpCodes.Ldfld, ovcRef));
        il.Append(il.Create(OpCodes.Ldarg_0));
        il.Append(il.Create(OpCodes.Ldftn, OnChanged_Ref));
        il.Append(il.Create(OpCodes.Newobj, actionRef));
        il.Append(il.Create(OpCodes.Call, RemoveRef));
        il.Append(il.Create(OpCodes.Castclass, actionTypeRef));
        il.Append(il.Create(OpCodes.Stfld, ovcRef));
    }

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

    public static TypeReference MakeGenericType(this TypeReference self, params TypeReference[] arguments)
    {
        if (self.GenericParameters.Count != arguments.Length)
            throw new ArgumentException();

        var instance = new GenericInstanceType(self);
        foreach (var argument in arguments)
            instance.GenericArguments.Add(argument);

        return instance;
    }

    public static MethodReference MakeGeneric(this MethodReference self, params TypeReference[] arguments)
    {
        var reference = new MethodReference(self.Name, self.ReturnType)
        {
            DeclaringType = self.DeclaringType.MakeGenericType(arguments),
            HasThis = self.HasThis,
            ExplicitThis = self.ExplicitThis,
            CallingConvention = self.CallingConvention,
        };

        foreach (var parameter in self.Parameters)
            reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));

        foreach (var generic_parameter in self.GenericParameters)
            reference.GenericParameters.Add(new GenericParameter(generic_parameter.Name, reference));

        return reference;
    }

    public static FieldReference MakeGeneric(this FieldReference self, params TypeReference[] arguments)
    {
        return new FieldReference(self.Name, self.FieldType, self.DeclaringType.MakeGenericType(arguments));
    }


    // [UnityEditor.Callbacks.DidReloadScripts]
    public static void OnPressInject()
    {
        if (!EditorApplication.isPlaying)
        {
            ProcessAssembly();
        }
    }
}