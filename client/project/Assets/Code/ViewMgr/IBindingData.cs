namespace Code.ViewMgr
{
    public interface IBindingData
    {
        object BindingData { get; set; }
        int GetCfgId();
        void UpdateUI();
    }
}