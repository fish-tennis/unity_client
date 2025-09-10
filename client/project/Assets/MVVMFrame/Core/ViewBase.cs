using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public abstract class ViewBase<T, T2> : MonoBehaviour where T : ViewModelBase<T2> where T2 : ModelBase, new()
{
    protected bool _isInit;
    public T _viewModel;
    public UIElements UIElements;

    public T BindingContext
    {
        get => _viewModel;
        set
        {
            if (!_isInit)
            {
                _viewModel = value;
                OnInitialize();
                BindProperty();
                _isInit = true;
            }
        }
    }


    public virtual void BindProperty()
    {
#if UNITY_EDITOR
        foreach (var field in typeof(T).GetFields())
        {
            if (field.FieldType.GenericTypeArguments.Length == 1)
            {
                var agrtype = field.FieldType.GenericTypeArguments[0];
                var tType = typeof(BindableProperty<>).MakeGenericType(agrtype);
                if (field.FieldType == tType)
                {
                    var method = GetType().GetMethod($"OnChanged_{field.Name}", new Type[] {agrtype, agrtype});
                    if (method != null)
                    {
                        var bindVal = field.GetValue(_viewModel);
                        var ovc = field.FieldType.GetField("OnValueChanged");
                        var dmethod = Delegate.CreateDelegate(ovc.FieldType, this, method);
                        ovc.SetValue(bindVal, dmethod);
                    }
                }
            }
        }
#endif
    }

    public virtual void UnBindProperty()
    {
#if UNITY_EDITOR
        foreach (var field in typeof(T).GetFields())
        {
            if (field.FieldType.GenericTypeArguments.Length == 1)
            {
                var agrtype = field.FieldType.GenericTypeArguments[0];
                var tType = typeof(BindableProperty<>).MakeGenericType(agrtype);
                if (field.FieldType == tType)
                {
                    var method = GetType().GetMethod($"OnChanged_{field.Name}", new Type[] {agrtype, agrtype});
                    if (method != null)
                    {
                        var bindVal = field.GetValue(_viewModel);
                        var ovc = field.FieldType.GetField("OnValueChanged");
                        ovc.SetValue(bindVal, null);
                    }
                }
            }
        }
#endif
    }

    public virtual void OnInitialize()
    {
    }

    private void OnDestroy()
    {
        Destroy();
        BindingContext?.OnDestroy();
    }
    
    public virtual void OnOpen()
    {
    }

    public virtual void OnClose()
    {
    }

    public virtual void Destroy()
    {
    }

    public virtual void SendCommond(string name, object[] arg = null)
    {
#if UNITY_EDITOR
        name = "OnCommond_" + name;
        foreach (var method in typeof(T2).GetMethods())
        {
            if (method.Name == name)
            {
                method.Invoke(BindingContext._model, arg);
                return;
            }
        }
#endif
    }

    public virtual void SendCommond<T1>(string name,T1 arg)
    {
        if (name == "SetTime")
        {
            var model = _viewModel._model as TestModel;
            model.OnCommond_SetTime(1);
        }
    }
    
    public virtual void SendCommond<T1,T2>(string name,T1 arg,T2 arg2)
    {
        
    }
}