namespace Code.Controls
{
    // 挂在控件上的脚本接口
    public interface IControlScript<TData,TKey>
    {
        // 绑定的数据
        TData BindingData { get; set; }
        
        // 控件标识(如可以是配置id或者唯一id等)
        TKey GetKey();
        
        // 刷新ui
        void UpdateUI();
    }
}