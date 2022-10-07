using Common.NetworkingData;
using DarkRift;
using DarkRift.Client;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility.Debugging;

namespace Client
{

    public class LoginManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private InputField nameInput;

        [SerializeField]
        private Button submitLoginButton;

        void Start()
        {
            Invoke(nameof(DelayedStart), .5f);
        }

        private void DelayedStart()
        {
            if (CommandLineArgsHelper.GetUserNameFromCL(out string username))
            {
                nameInput.text = username;
                submitLoginButton.enabled = false;
                OnSubmitLogin();
            }
            else if (Application.isEditor)
            {
                nameInput.text = "test";
                submitLoginButton.enabled = false;
                OnSubmitLogin();
            }

            if (CommandLineArgsHelper.DebugUtilityEnabled())
            {
                Instantiate(Resources.Load("Prefabs\\Utility\\DebugUtility"));
            }

            submitLoginButton.onClick.AddListener(OnSubmitLogin);

            ConnectionManager.Instance.OnConnected += StartLoginProcess;
            ConnectionManager.Instance.Client.MessageReceived += OnMessage;
        }

        void OnDestroy()
        {
            ConnectionManager.Instance.OnConnected -= StartLoginProcess;
            ConnectionManager.Instance.Client.MessageReceived += OnMessage;
        }

        public void StartLoginProcess()
        {
            Debug.Log("Connected to server");
        }

        public void OnSubmitLogin()
        {
            if (!string.IsNullOrEmpty(nameInput.text))
            {
                using (Message message = Message.Create((ushort)NetworkingTags.LoginRequest, new LoginRequestData(nameInput.text)))
                {
                    ConnectionManager.Instance.Client.SendMessage(message, SendMode.Reliable);
                }
            }
        }

        private void OnMessage(object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage())
            {
                switch ((NetworkingTags)message.Tag)
                {
                    case NetworkingTags.LoginRequestDenied:
                        OnLoginDecline();
                        break;
                    case NetworkingTags.LoginRequestAccepted:
                        OnLoginAccept(message.Deserialize<LoginInfoData>());
                        break;
                }
            }
        }

        private void OnLoginDecline()
        {
            Debug.LogError("Login declined");
        }

        private void OnLoginAccept(LoginInfoData data)
        {
            ConnectionManager.Instance.PlayerId = data.Id;

            if (Application.isEditor && GameObject.Find("SinglePlayerServer") != null)
            {
                SceneManager.LoadScene("Client", LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync("Login");
            }
            else
            {
                SceneManager.LoadScene("Client");
            }
        }
    }

}
