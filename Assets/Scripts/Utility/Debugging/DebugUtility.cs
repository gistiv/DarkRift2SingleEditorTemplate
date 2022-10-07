using UnityEngine;
using Utility.Debugging.Console;

namespace Utility.Debugging
{
	
	public class DebugUtility : MonoBehaviour
	{
		public static DebugUtility Instance;

		public bool DebugConsoleOverlayActive = false;

		private DebugConsole console;

		void Awake()
	    {
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;

			console = GetComponent<DebugConsole>();
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftControl)) &&Input.GetKeyDown(KeyCode.C))
            {
				DebugConsoleOverlayActive = !DebugConsoleOverlayActive;

				console.DisplayConsole = DebugConsoleOverlayActive;

				if (DebugConsoleOverlayActive)
                {
					Cursor.lockState = CursorLockMode.None;
				}
                else
                {
					Cursor.lockState = CursorLockMode.Locked;
				}
			}
	    }
	}
	
}
