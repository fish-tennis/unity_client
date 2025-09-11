using System;

public class BindableProperty<T>
{
    public Action<T, T> OnValueChanged;
    private T _value;

    public T Value
    {
        get => _value;
        set
        {
            if (!Equals(_value, value))
            {
                T old = _value;
                _value = value;
                OnValueChanged?.Invoke(old, _value);
            }
        }
    }

    public BindableProperty()
    {
    }

    public BindableProperty(T val)
    {
        _value = val;
    }

    public void Sync()
    {
        OnValueChanged?.Invoke(_value, _value);
    }

    public override string ToString()
    {
        return (Value != null ? Value.ToString() : "null");
    }
}