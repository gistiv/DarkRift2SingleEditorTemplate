using System;

namespace Utility.Debugging
{

    public static class CommandLineArgsHelper
	{
		public static bool GetUserNameFromCL(out string username)
        {
			string[] args = Environment.GetCommandLineArgs();

			for (int i = 0; i < args.Length; i++)
			{
				if (args[i].Contains("-username"))
                {
					username = args[i + 1];
					return true;
                }
            }

			username = string.Empty;
			return false;
        }

		public static bool DebugUtilityEnabled()
		{
			string[] args = Environment.GetCommandLineArgs();

			for (int i = 0; i < args.Length; i++)
			{
				if (args[i].Contains("-debugUtil"))
				{
					return true;
				}
			}

			return false;
		}
	}
	
}
