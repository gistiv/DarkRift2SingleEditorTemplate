using UnityEngine;
using DarkRift;
using DarkRift.Client.Unity;
using System;
using System.Net;

namespace Client
{

    [RequireComponent(typeof(UnityClient))]
    public class ConnectionManager : MonoBehaviour
    {
        public UnityClient Client { get; private set; }

        public static ConnectionManager Instance;

        public ushort PlayerId { get; set; }

        public delegate void OnConnectedDelegate();

        public event OnConnectedDelegate OnConnected;

        [Header("Settings")]
        [SerializeField]
        private string ipAdress;

        [SerializeField]
        private int port;


        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this);

            Client = GetComponent<UnityClient>();
        }

        private void Start()
        {
            Client.ConnectInBackground(IPAddress.Parse(ipAdress), port, false, ConnectCallback);
        }

        private void ConnectCallback(Exception exception)
        {
            if (Client.ConnectionState == ConnectionState.Connected)
            {
                OnConnected?.Invoke();
            }
            else
            {
                Debug.LogError("Unable to connect to server.");
            }
        }
    }

}
