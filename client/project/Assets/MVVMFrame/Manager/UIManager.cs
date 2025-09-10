using System;
using System.Collections;
using System.Collections.Generic;
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

    public T CreateModel<T>() where T : ModelBase, new()
    {
        T model = new T();
        model.Initialization();
        modelDict.Add(typeof(T), model);
        
        return model;
    }
}