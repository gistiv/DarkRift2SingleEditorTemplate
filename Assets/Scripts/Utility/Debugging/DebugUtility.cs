using UnityEngine;
using Utility.Debugging.Console.Input;
using Utility.Debugging.Console.Log;

namespace Utility.Debugging
{
	[RequireComponent(typeof(DebugConsole))]
	[RequireComponent(typeof(DebugConsoleInput))]
	public class DebugUtility : MonoBehaviour
	{
		public static DebugUtility Instance;

		public bool DebugConsoleOverlayActive = false;

		public bool ToogleFromInput = false;

		private DebugConsole console;
		private DebugConsoleInput input;


		void Awake()
	    {
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(this);

			console = GetComponent<DebugConsole>();
			input = GetComponent<DebugConsoleInput>();
	    }

	    // Update is called once per frame
	    void Update()
	    {
            if (ToogleFromInput || (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.C))
            {
				DebugConsoleOverlayActive = !DebugConsoleOverlayActive;
				ToogleFromInput = false;

				console.DisplayConsole = DebugConsoleOverlayActive;
				input.DisplayConsoleInput = DebugConsoleOverlayActive;

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

		public void ToggleDebugConsole()
        {

		}

		public void RegisterAsServer()
        {
			input.RegisterServerCommands();
        }

		public void RegisterAsClient()
		{
			input.RegisterClientCommands();
		}
	}
	
}
