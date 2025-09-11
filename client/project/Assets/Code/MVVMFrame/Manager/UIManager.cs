using System;
using System.Collections;
using System.Collections.Generic;
using cshap_client.network;
using UnityEngine;

public class UIManager
{
    private static UIManager _ins;
    public static UIManager Instance => _ins ??= new UIManager();
    private readonly List<ModelBase> allModel = new List<ModelBase>();
    private readonly Dictionary<Type, ModelBase> modelDict = new Dictionary<Type, ModelBase>();

    public void UnInitialize()
    {
        modelDict.Clear();
    }

    public T GetModel<T>() where T : ModelBase, new()
    {
        if (modelDict.ContainsKey(typeof(T)))
        {
            return modelDict[typeof(T)] as T;
        }

        return CreateModel<T>();
    }

    public ModelBase GetModelByType(Type type)
    {
        return modelDict.ContainsKey(type) ? modelDict[type] : null;
    }

    public T CreateModel<T>() where T : ModelBase, new()
    {
        T model = new T();
        Debug.Log("CreateModel:" + model.GetType().Name +" typeof(T):" + typeof(T).Name);
        HandlerRegister.RegisterMethodsForClass(model.GetType(), "");
        model.Initialization();
        modelDict.Add(typeof(T), model);
        
        return model;
    }
}