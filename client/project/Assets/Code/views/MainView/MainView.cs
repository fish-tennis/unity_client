using Code.ViewMgr;
using cshap_client.game;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;

namespace Code.views.MainView
{
    // 主界面
    public class MainView : ViewBase
    {
        public InputField InputField_Cmd;
        public Button Button_Cmd;
        
        public void OnClickCmd()
        {
            string cmd = InputField_Cmd.text.Trim();
            if (string.IsNullOrEmpty(cmd))
            {
                Debug.LogError("cmd is empty");
                return;
            }
            Client.Send(new Gserver.TestCmd
            {
                Cmd = cmd,
            });
        }
    }
}