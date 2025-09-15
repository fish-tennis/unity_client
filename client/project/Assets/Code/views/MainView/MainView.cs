using Code.game;
using Code.ViewMgr;
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
            if (cmd[0] == '@')
            {
                if(Client.Send(new Gserver.TestCmd
                {
                    Cmd = cmd[1..],
                }))
                {
                    InputField_Cmd.text = "@";
                }
            }
            else
            {
                Debug.LogError("test cmd must start with @");
            }
        }

        // 切换到任务界面
        public void OnClickQuest()
        {
            gameObject.SetActive(false);
            gameObject.transform.parent.Find("QuestView").gameObject.SetActive(true);
        }
    }
}