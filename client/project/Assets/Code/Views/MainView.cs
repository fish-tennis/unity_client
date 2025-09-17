using Code.game;
using Code.ViewMgr;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Views
{
    // 主界面
    public class MainView : ViewBase
    {
        [SerializeField] private Text Text_PlayerName;
        [SerializeField] private Text Text_PlayerLevel;
        [SerializeField] private Text Text_PlayerExp;
        [SerializeField] private InputField InputField_Cmd;
        [SerializeField] private Button Button_Cmd;
        [SerializeField] private Button Button_Bag;
        [SerializeField] private Button Button_Quest;
        [SerializeField] private Button Button_Activity;

        public void Start()
        {
            Button_Cmd.onClick.AddListener(OnClickCmd);
            Button_Bag.onClick.AddListener(OnClickBag);
            Button_Quest.onClick.AddListener(OnClickQuest);
            Button_Activity.onClick.AddListener(OnClickActivity);
            
            ShowView("MainView");
            UpdatePlayerInfo();
        }

        // 显示指定view并隐藏其他view
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
        
        // 切换到背包界面
        public void OnClickBag()
        {
            Debug.LogError("背包界面正在制作中");
            //;ShowView("BagView");
        }

        // 切换到任务界面
        public void OnClickQuest()
        {
            ShowView("QuestView");
        }
        
        // 切换到活动界面
        public void OnClickActivity()
        {
            ShowView("ActivityView");
        }

        // 基础信息更新
        public void OnBaseInfoSync(Gserver.BaseInfoSync res)
        {
            UpdatePlayerInfo();
        }

        private void UpdatePlayerInfo()
        {
            var data = Client.Instance.Player.BaseInfo.data;
            if (data == null)
            {
                return;
            }
            Text_PlayerName.text = Client.Instance.Player.Name;
            Text_PlayerLevel.text = $"Lv.{data.Level}";
            Text_PlayerExp.text = data.Exp.ToString();
        }
    }
}