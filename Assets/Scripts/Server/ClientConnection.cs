using Common.NetworkingData;
using DarkRift;
using DarkRift.Server;

namespace Server
{

    public class ClientConnection
	{
        public string Name { get; }

        public IClient Client { get; }

        public ServerInstance ServerInstance;

        public PlayerServer Player { get; set; }

        public ClientConnection(IClient client, LoginRequestData data, ServerInstance serverInstance)
        {
            Client = client;
            Name = data.Name;
            ServerInstance = serverInstance;

            ServerManager.Instance.Players.Add(client.ID, this);
            ServerManager.Instance.PlayersByName.Add(Name, this);

            using (Message m = Message.Create((ushort)NetworkingTags.LoginRequestAccepted, new LoginInfoData(client.ID)))
            {
                client.SendMessage(m, SendMode.Reliable);
            }

            Client.MessageReceived += OnMessage;
        }

        private void OnMessage(object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage())
            {
                switch ((NetworkingTags)message.Tag)
                {
                    case NetworkingTags.GameJoinRequest:
                        ServerInstance.JoinPlayerToGame(this);
                        break;
                    case NetworkingTags.GamePlayerInput:
                        Player.RecieveInput(message.Deserialize<PlayerInputData>());
                        break;
                }
            }
        }

        public void OnClientDisconnect(object sender, ClientDisconnectedEventArgs e)
        {
            ServerInstance.RemovePlayer(this);
            ServerManager.Instance.Players.Remove(Client.ID);
            ServerManager.Instance.PlayersByName.Remove(Name);
            e.Client.MessageReceived -= OnMessage;
        }
    }
	
}
