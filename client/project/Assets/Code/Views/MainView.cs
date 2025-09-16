using Code.game;
using Code.ViewMgr;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;

namespace Code.Views
{
    // 主界面
    public class MainView : ViewBase
    {
        public InputField InputField_Cmd;
        public Button Button_Cmd;

        public void Start()
        {
            base.Start();
            ShowView("MainView");
        }

        public void ShowView(string viewName)
        {
            Debug.Log($"ShowView:{viewName}");
            for (var i = 0; i < gameObject.transform.parent.childCount; i++)
            {
                var viewNode = gameObject.transform.parent.GetChild(i);
                viewNode.gameObject.SetActive(viewNode.name == viewName);
            }
        }
        
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
            ShowView("QuestView");
        }
    }
}