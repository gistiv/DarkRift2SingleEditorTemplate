using Common;
using Common.NetworkingData;
using Common.Player;
using DarkRift;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Client
{
    [RequireComponent(typeof(PlayerInterpolation))]
    [RequireComponent(typeof(PlayerLogic))]
    public class PlayerClient : MonoBehaviour
    {
        public PlayerInterpolation interpolation; 

        private PlayerLogic playerLogic;

        // Store look direction.
        private float yaw;
        private float pitch;

        private ushort id;
        private string playerName;
        private bool isOwn;

        private Queue<ReconciliationInfo> reconciliationHistory = new Queue<ReconciliationInfo>();

        [Header("Settings")]
        [SerializeField]
        private float sensitivityX = 5;
        [SerializeField]
        private float sensitivityY = -5;

        void Awake()
        {
            playerLogic = GetComponent<PlayerLogic>();
            interpolation = GetComponent<PlayerInterpolation>();
        }

        public void Initialize(ushort id, string playerName)
        {
            this.id = id;
            this.playerName = playerName;

            if (ConnectionManager.Instance.PlayerId == id)
            {
                isOwn = true;
                Camera.main.transform.SetParent(transform);
                Camera.main.transform.localPosition = new Vector3(0, 1.5f, 0);
                Camera.main.transform.localRotation = Quaternion.identity;
                interpolation.CurrentData = new PlayerStateData(this.id, 0, Vector3.zero, 0f, 0f);
            }
        }

        void FixedUpdate()
        {
            if (isOwn)
            {
                bool[] inputs = new bool[7];
                inputs[0] = Input.GetKey(KeyCode.W);
                inputs[1] = Input.GetKey(KeyCode.A);
                inputs[2] = Input.GetKey(KeyCode.S);
                inputs[3] = Input.GetKey(KeyCode.D);
                inputs[4] = Input.GetKey(KeyCode.Space);
                inputs[5] = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                inputs[6] = Input.GetMouseButton(0);

                yaw += Input.GetAxis("Mouse X") * sensitivityX;
                pitch += Input.GetAxis("Mouse Y") * sensitivityY;

                PlayerInputData inputData = new PlayerInputData(inputs, yaw, pitch, GameManager.Instance.LastReceivedServerTick - 1U);

                transform.position = interpolation.CurrentData.Position;
                PlayerStateData nextStateData = playerLogic.GetNextFrameData(inputData, interpolation.CurrentData);
                interpolation.SetFramePosition(nextStateData);

                using (Message message = Message.Create((ushort)NetworkingTags.GamePlayerInput, inputData))
                {
                    ConnectionManager.Instance.Client.SendMessage(message, SendMode.Reliable);
                }

                reconciliationHistory.Enqueue(new ReconciliationInfo(GameManager.Instance.ClientTick, nextStateData, inputData));
            }
        }

        public void OnServerDataUpdate(PlayerStateData playerStateData)
        {
            if (isOwn)
            {
                while (reconciliationHistory.Any() && reconciliationHistory.Peek().Frame < GameManager.Instance.LastReceivedServerTick)
                {
                    reconciliationHistory.Dequeue();
                }

                if (reconciliationHistory.Any() && reconciliationHistory.Peek().Frame == GameManager.Instance.LastReceivedServerTick)
                {
                    ReconciliationInfo info = reconciliationHistory.Dequeue();
                    if (Vector3.Distance(info.Data.Position, playerStateData.Position) > 0.05f)
                    {

                        List<ReconciliationInfo> infos = reconciliationHistory.ToList();
                        interpolation.CurrentData = playerStateData;
                        transform.position = playerStateData.Position;
                        transform.rotation = Quaternion.Euler(0.0f, playerStateData.Yaw, 0.0f);
                        for (int i = 0; i < infos.Count; i++)
                        {
                            PlayerStateData u = playerLogic.GetNextFrameData(infos[i].Input, interpolation.CurrentData);
                            interpolation.SetFramePosition(u);
                        }
                    }
                }
            }
            else
            {
                interpolation.SetFramePosition(playerStateData);
            }
        }
    }

}
