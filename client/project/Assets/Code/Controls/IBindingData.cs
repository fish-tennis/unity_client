namespace Code.Controls
{
    // 绑定数据接口
    public interface IBindingData<T>
    {
        // 绑定的数据
        T BindingData { get; set; }
        
        // 对应的配置id(可选项)
        int GetCfgId();
        
        // 刷新ui
        void UpdateUI();
    }
}