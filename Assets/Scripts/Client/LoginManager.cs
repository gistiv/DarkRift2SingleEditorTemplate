using System;
using Common;
using Common.NetworkingData;
using DarkRift;
using DarkRift.Client;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Client
{

    public class LoginManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private TMP_Text nameInput;

        [SerializeField]
        private Button submitLoginButton;

        void Start()
        {
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
            // todo: error screnn with reason why declined
        }

        private void OnLoginAccept(LoginInfoData data)
        {
            ConnectionManager.Instance.PlayerId = data.Id;
            SceneManager.LoadScene("Client");
        }
    }

}
