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
        public string Host;
        [SerializeField]
        public int Port;


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
            IPAddress ip = new IPAddress(new byte[]{ 127,0,0,1});

            if (IPAddress.TryParse(Host, out var address))
            {
                switch (address.AddressFamily)
                {
                    // ipv4 or ipv6
                    case System.Net.Sockets.AddressFamily.InterNetwork:
                    case System.Net.Sockets.AddressFamily.InterNetworkV6:
                        ip = IPAddress.Parse(Host);
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            else
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(Host);

                if (hostEntry.AddressList.Length > 0)
                {
                    ip = hostEntry.AddressList[0];
                }
            }

            Client.ConnectInBackground(ip, Port, true, ConnectCallback);
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
