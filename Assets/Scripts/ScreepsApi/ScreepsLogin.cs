using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ScreepsViewer
{
    public class ScreepsLogin : MonoBehaviour
    {
        [SerializeField] private ScreepsAPI api;
        [SerializeField] private Toggle save;
        [SerializeField] private Toggle ssl;
        [SerializeField] private TMP_InputField server;
        [SerializeField] private TMP_InputField port;
        [SerializeField] private TMP_InputField email;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private Button connect;
        [SerializeField] private Button exit;
        public FadePanel loginPanel;
        public FadePanel logingInPanel;
        public FadePanel autoLoginNotice;
        public TMP_Text autoLoginNoticeText;
        public TMP_Text loggingInPanelText;
        public static bool cancelingLogin = false;
        public Action<Credentials, Address> OnSubmit;
        public string secret = "abc123";

        private void Start()
        {
            autoLoginNotice.Show(false, true);
            PlayerPrefs.SetInt("saveCredentials", 0);
            // TODO : add a check to see if there is a token for this device or if there is a saved login
            connect.onClick.AddListener(OnClick);
            //Debug.Log("on click added");
            exit.onClick.AddListener(QuitApplication);
            UpdateSaveSetting(true);
            save.onValueChanged.AddListener(UpdateSaveSetting);
            RefreshSavedSettings();
            api.OnConnectionStatusChange += OnConnectionStatusChange;
            api.PrintConnectionStatus += PrintConnectionStatus;
            gameObject.GetComponent<ScreepsHTTP>().PrintConnectionStatus += PrintConnectionStatus;
            gameObject.GetComponent<ScreepsSocket>().PrintConnectionStatus += PrintConnectionStatus;
            loginPanel.Show(false, true);
            loginPanel.Show(); // fade in
            logingInPanel.Show(false, true);
            logingInPanel.Show();
        }
        public void QuitApplication()
        {
            Application.Quit();
        }
        private void OnConnectionStatusChange(bool isConnected)
        {
            if (isConnected)
            {
                StartCoroutine(showLogingInPanel());
                //logingInPanel.gameObject.SetActive(false);
                //loginPanel.gameObject.SetActive(false);
            } else
            {
                logingInPanel.Show(true);
                //logingInPanel.gameObject.SetActive(true);
                //loginPanel.gameObject.SetActive(true);
            }
        }
        IEnumerator showLogingInPanel()
        {
            yield return new WaitForSeconds(3f);
            logingInPanel.Show(false);
            SceneManager.LoadScene(1);
        }
        private void PrintConnectionStatus(string statusUpdate)
        {
            loggingInPanelText.text += System.Environment.NewLine + statusUpdate;
        }
        
        private void RefreshSavedSettings()
        {
            if (PlayerPrefs.GetInt("saveCredentials") == 1)
                save.isOn = true;
            else
            {
                save.isOn = false;
                cancelingLogin = true;
            }
            //Debug.Log(string.Format("save value: {0}", save));
            var originalPWinput = this.passwordInput.text;
            if (save.isOn && this.passwordInput.text.Length > 0)
            {
                //this.save.isOn = true;
                var port = PlayerPrefs.GetString("port");
                if (port != null)
                {
                    this.server.text = port;
                }


                var server = PlayerPrefs.GetString("server");
                if (server != null)
                {
                    this.server.text = server;
                }
                var email = PlayerPrefs.GetString("email");
                if (email != null)
                {
                    this.email.text = email;
                }
                var encryptedPassword = PlayerPrefs.GetString("password");
                var password = Crypto.DecryptStringAES(encryptedPassword, secret);
                var ssl = PlayerPrefs.GetInt("ssl");
                //this.ssl.isOn = ssl == 1;
                this.passwordInput.text = password;
            }
            if (this.passwordInput.text != originalPWinput && !cancelingLogin)
            {
                try
                {
                    StartCoroutine(loginWait());
                    SwitchToCancelLogin();
                }
                catch (Exception ex)
                {
                    if (ex is FormatException || ex is OverflowException)
                    {
                        //WebId = Guid.Empty;
                        return;
                    }
                    throw;
                }
            }
        }
        private void SwitchToCancelLogin()
        {
            //Debug.Log("switched to cancel login");
            connect.gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Cancel Login";
            connect.onClick.RemoveAllListeners();
            connect.onClick.AddListener(CancelLogin);
        }
        private void SwitchToLogin()
        {
            autoLoginNotice.Show(false);
            connect.gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Login";
            //Debug.Log("switched to login");
            connect.onClick.RemoveAllListeners();
            connect.onClick.AddListener(OnClick);
        }
        private void CancelLogin()
        {
            //Debug.Log("cancel clicked");
            cancelingLogin = true;
            StopAllCoroutines();
            SwitchToLogin();
        }
        IEnumerator loginWait()
        {
            autoLoginNotice.Show(true,true);
            ChangeAutoLoginText(5);
            yield return new WaitForSeconds(1f);
            ChangeAutoLoginText(4);
            yield return new WaitForSeconds(1f);
            ChangeAutoLoginText(3);
            yield return new WaitForSeconds(1f);
            ChangeAutoLoginText(2);
            yield return new WaitForSeconds(1f);
            ChangeAutoLoginText(1);
            yield return new WaitForSeconds(1f);
            if (!cancelingLogin)
                OnClick();
        }

        private void ChangeAutoLoginText(int num)
        {
            autoLoginNoticeText.text = "Login info saved" + Environment.NewLine + num.ToString() + " seconds until login";
        }

        private void UpdateSaveSetting(bool value)
        {
            PlayerPrefs.SetInt("saveCredentials", value ? 1 : 0);
            if (!value)
            {
                PlayerPrefs.SetString("email", "");
                PlayerPrefs.SetString("password", "");
            }
        }

        private void OnClick()
        {
            cancelingLogin = false;
            if (save.isOn)
            {
                PlayerPrefs.SetString("port", port.text);
                PlayerPrefs.SetString("server", server.text);
                PlayerPrefs.SetString("email", email.text);
                var password = this.passwordInput.text;
                var encryptedPassword = Crypto.EncryptStringAES(password, secret);
                PlayerPrefs.SetString("password", encryptedPassword);
                //PlayerPrefs.SetInt("ssl", ssl.isOn ? 1 : 0);
            }

            var credentials = new Credentials
            {
                email = email.text,
                password = passwordInput.text
            };
            var address = new Address
            {
                hostName = server.text,
                path = "/",
                ssl = true, 
                port = port.text
            };
            api.Connect(credentials, address);
            loginPanel.Show(false);
            logingInPanel.Show(true,true);
        }
    }

    public struct Credentials
    {
        public string email;
        public string password;
    }

    public struct Address
    {
        public bool ssl;
        public string hostName;
        public string port;
        public string path;

        public string Http(string path = "")
        {
            if (path.StartsWith("/") && this.path.EndsWith("/"))
            {
                path = path.Substring(1);
            }
            var protocol = ssl ? "https" : "http";
            var port = hostName.ToLowerInvariant() == "screeps.com" ? "" : string.Format(":{0}", this.port);
            Debug.Log(string.Format("{0}://{1}{2}{3}{4}", protocol, hostName, port, this.path, path));
            return string.Format("{0}://{1}{2}{3}{4}", protocol, hostName, port, this.path, path);
        }
    }
}