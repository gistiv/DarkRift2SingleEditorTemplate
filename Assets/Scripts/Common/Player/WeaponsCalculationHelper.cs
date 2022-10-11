using UnityEngine;

namespace Common.Player
{
	
	public static class WeaponsCalculationHelper
	{
		public static uint CaclulateShootCooldownFrame(uint currentFrame)
        {
			// this value is depended on the weapon and should therefore come from a scriptable object
			float fireRate = 0.5f;
			return currentFrame + (uint)(fireRate / Time.fixedDeltaTime);
		}
	}
	
}
