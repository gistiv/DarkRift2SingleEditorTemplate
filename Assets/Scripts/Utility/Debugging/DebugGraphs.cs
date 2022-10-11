using Client;
using System.Net.NetworkInformation;
using UnityEngine;

namespace Utility.Debugging
{
	
	public class DebugGraphs : MonoBehaviour
	{
        private DebugGUI debugGui;

        private bool isClient = false;

        private void Start()
        {
            debugGui = GetComponent<DebugGUI>();
        }

        private void Update()
        {
            float fps = (1 / Time.unscaledDeltaTime);
            float smoothFps = (1 / Time.smoothDeltaTime);

            DebugGUI.SetGraphProperties("avgFrameRate", "FPS: " + fps.ToString("F0"), 0, 80, 0, new Color(1, 0.3f, 0.3f), true);
            DebugGUI.SetGraphProperties("avgFrameRateSmooth", "SmoothFPS: " + smoothFps.ToString("F0"), 0, 80, 0, new Color(0, 1, 0), true);

            DebugGUI.Graph("avgFrameRate", fps);
            DebugGUI.Graph("avgFrameRateSmooth", smoothFps);

            if (isClient)
            {
                System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
                PingReply reply = pingSender.Send(ConnectionManager.Instance.Host, 100);

                if (reply.Status == IPStatus.Success)
                {
                    float latencyPing = reply.RoundtripTime;

                    DebugGUI.SetGraphProperties("latencyPing", "Latency: " + latencyPing.ToString("F0") + "ms", 0, 250, 0, new Color(1f, 1f, 1f), true);
                    DebugGUI.Graph("latencyPing", latencyPing);
                }
            }
        }

        public void RegisterAsClient()
        {
            isClient = true;
        }

        private void OnEnable()
        {
            if(debugGui != null)
            {
                debugGui.enabled = true;
            }
        }

        private void OnDisable()
        {
            if (debugGui != null)
            {
                debugGui.enabled = false;
            }
        }
    }
	
}
