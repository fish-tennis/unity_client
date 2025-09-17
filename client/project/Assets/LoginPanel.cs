using System;
using Code.game;
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
        host.text = PlayerPrefs.GetString("Host", "");
        account.text = PlayerPrefs.GetString("AccountName", "");
    }

    private void Update()
    {

    }

    void OnClick()
    {
        Login.s_AccountName = account.text.Trim();
        Login.s_Password = password.text.Trim();
        Text_Error.text = "connecting to " + host.text;
        if (!Client.Instance.Connect(host.text.Trim()))
        {
            Text_Error.text = "连接失败";
            return;
        }
        PlayerPrefs.SetString("Host", host.text.Trim());
        PlayerPrefs.SetString("AccountName", Login.s_AccountName);
        // PlayerPrefs.SetString("Password", Login.GetMd5Password(Login.s_Password));
    }
    // Update is called once per frame
}
