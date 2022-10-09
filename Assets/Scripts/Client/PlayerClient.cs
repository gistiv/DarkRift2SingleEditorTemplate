using Common.NetworkingData;
using Common.Player;
using DarkRift;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility.Debugging;

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

        private Camera playerCamera;
        private Transform eyes;

        [Header("Settings")]
        [SerializeField]
        private float sensitivityX = 3;
        [SerializeField]
        private float sensitivityY = -3;

        void Awake()
        {
            playerLogic = GetComponent<PlayerLogic>();
            interpolation = GetComponent<PlayerInterpolation>();
            
            eyes = gameObject.transform.Find("Eyes");
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
                playerCamera = Camera.main;

                Cursor.lockState = CursorLockMode.Locked;

                interpolation.CurrentData = new PlayerStateData(this.id, 0, Vector3.zero, 0f, 0f);
            }
        }

        void FixedUpdate()
        {
            if (!isOwn)
            {
                return;
            }

            bool[] inputs = new bool[7];

            // disable player input -> when debug console open, when dead or during cutscene/animation
            if (DebugUtility.Instance != null && DebugUtility.Instance.DebugConsoleOverlayActive)
            {
                inputs[0] = false;
                inputs[1] = false;
                inputs[2] = false;
                inputs[3] = false;
                inputs[4] = false;
                inputs[5] = false;
                inputs[6] = false;
            }
            // player input
            else
            {
                inputs[0] = Input.GetKey(KeyCode.W);
                inputs[1] = Input.GetKey(KeyCode.A);
                inputs[2] = Input.GetKey(KeyCode.S);
                inputs[3] = Input.GetKey(KeyCode.D);
                inputs[4] = Input.GetKey(KeyCode.Space);
                inputs[5] = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                inputs[6] = Input.GetMouseButton(0);

                yaw += Input.GetAxis("Mouse X") * sensitivityX;
                pitch += Input.GetAxis("Mouse Y") * sensitivityY;

                if (pitch >= 35.0)
                {
                    pitch = 35f;
                }
                else if (pitch <= -55.0)
                {
                    pitch = -55f;
                }

                playerCamera.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
            }

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
                eyes.rotation = Quaternion.Euler(playerStateData.Pitch, playerStateData.Yaw, 0);
            }
        }
    }

}
