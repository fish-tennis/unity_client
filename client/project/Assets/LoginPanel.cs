using System;

using cshap_client.game;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    public InputField host;
    public InputField account;
    public InputField password;
    public Text Text_Error;

    public Button btn;
    
    // Start is called before the first frame update
    private void Awake()
    {
        
    }

    void Start()
    {
        btn.onClick.AddListener(OnClick);
        Login.LoginTipAction = (tip) =>
        {
            Text_Error.text = tip;
        };
    }

    private void Update()
    {

    }

    void OnClick()
    {
        Login.s_AccountName = account.text;
        Login.s_Password = password.text;
        Text_Error.text = "connecting to " + host.text;
        if (!Client.Instance.Connect(host.text))
        {
            Text_Error.text = "连接失败";
        }
    }
    // Update is called once per frame
}
