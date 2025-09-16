using UnityEngine;

namespace Code.Views
{
    public class Views : MonoBehaviour
    {
        public void Start()
        {
            // 显示MainView 隐藏其他view
            for (var i = 0; i < transform.childCount; i++)
            {
                var viewNode = transform.GetChild(i);
                viewNode.gameObject.SetActive(viewNode.name == "MainView");
            }
        }
    }
}