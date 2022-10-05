using Common.NetworkingData;
using UnityEngine;

namespace Client
{

    public class PlayerInterpolation : MonoBehaviour
	{
		private float lastInputTime;

		public PlayerStateData CurrentData { get; set; }
		public PlayerStateData PreviousData { get; private set; }

		// Start is called before the first frame update
		void Start()
	    {
	        
	    }

		// Update is called once per frame
		public void Update()
		{
			float timeSinceLastInput = Time.time - lastInputTime;
			float t = timeSinceLastInput / Time.fixedDeltaTime;
			transform.position = Vector3.LerpUnclamped(PreviousData.Position, CurrentData.Position, t);
			transform.rotation = Quaternion.SlerpUnclamped(Quaternion.Euler(0.0f, PreviousData.Yaw, 0.0f), Quaternion.Euler(0.0f, CurrentData.Yaw, 0.0f), t);
		}

		public void SetFramePosition(PlayerStateData data)
		{
			RefreshToPosition(data, CurrentData);
		}

		public void RefreshToPosition(PlayerStateData data, PlayerStateData prevData)
		{
			PreviousData = prevData;
			CurrentData = data;
			lastInputTime = Time.fixedTime;
		}
	}
	
}
