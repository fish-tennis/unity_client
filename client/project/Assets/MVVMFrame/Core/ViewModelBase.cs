using UnityEngine;
using System;

public class ViewModelBase<T> where T : ModelBase, new()
{
    public T _model;

    public ViewModelBase()
    {
        _model = UIManager.Instance.GetModel<T>();
        OnInitialize();
        BindProperty();
    }

    public virtual void OnInitialize()
    {
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

    public void OnDestroy()
    {
        Destroy();
        UnBindProperty();
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
                        var bindVal = field.GetValue(_model);
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
        foreach (var field in _model.GetType().GetFields())
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
                        var bindVal = field.GetValue(_model);
                        var ovc = field.FieldType.GetField("OnValueChanged");
                        ovc.SetValue(bindVal, null);
                    }
                }
            }
        }
#endif
    }
}