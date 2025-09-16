using System;
using System.Collections.Generic;
using Code.cfg;
using Code.game;
using Code.ViewMgr;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Views
{
    // 任务界面
    public class QuestView : ViewBase
    {
        [SerializeField] private Transform m_Content;
        [SerializeField]  private GameObject m_Template;
        private GameObject m_TemplateInstance;
        [SerializeField] private Button m_ButtonBack;

        public void Start()
        {
            m_ButtonBack.onClick.AddListener(OnClickBack);
            m_TemplateInstance = Instantiate(m_Template);
            Destroy(m_Template);
            UpdateQuests();
        }
        
        public void UpdateQuests()
        {
            var quest = Client.Instance.Player.GetQuest();
            BindingUtil.UpdateListView<Gserver.QuestData,QuestBindingData>(m_Content, m_TemplateInstance,
                quest.Quests,x=>x.CfgId);
        }

        public void OnClickBack()
        {
            Debug.Log("OnClickBack");
            ViewMgr.ViewMgr.Instance.GetViewByName<MainView>("MainView")?.ShowView("MainView");
        }

        public void OnQuestSync(Gserver.QuestSync res)
        {
            Debug.Log("OnQuestSync: Finished:" + res.Finished.Count + " Quests:" + res.Quests.Count);
            UpdateQuests();
        }

        // 任务完成的回调
        // 数据在Code.game.Quest.cs里面已经处理过了,这里只是刷新ui
        public void OnFinishQuestRes(Gserver.FinishQuestRes res)
        {
            Debug.Log("OnFinishQuestRes");
            UpdateQuests(); // 暂时不细化,整体刷新
        }

        // 任务删除的回调
        // 数据在Code.game.Quest.cs里面已经处理过了,这里只是刷新ui
        public void OnQuestRemoveRes(Gserver.QuestRemoveRes res)
        {
            Debug.Log("OnQuestRemoveRes");
            UpdateQuests(); // 暂时不细化,整体刷新
        }

        // 任务更新的回调
        // 数据在Code.game.Quest.cs里面已经处理过了,这里只是刷新ui
        public void OnQuestUpdate(Gserver.QuestUpdate res)
        {
            Debug.Log("OnQuestUpdate");
            UpdateQuests(); // 暂时不细化,整体刷新
        }
        
    }
}