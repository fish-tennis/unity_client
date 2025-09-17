namespace Code.Controls
{
    // 绑定数据接口
    public interface IBindingData<TData,TKey>
    {
        // 绑定的数据
        TData BindingData { get; set; }
        
        // 对应的配置id(可选项) TODO:改为GetKey
        TKey GetCfgId();
        
        // 刷新ui
        void UpdateUI();
    }
}