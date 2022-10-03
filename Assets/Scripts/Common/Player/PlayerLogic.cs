using Common.NetworkingData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Player
{
	[RequireComponent(typeof(CharacterController))]
	public class PlayerLogic : MonoBehaviour
	{
        private Vector3 gravity;

        [Header("Settings")]
        [SerializeField]
        private float walkSpeed;
        [SerializeField]
        private float runSpeed;
        [SerializeField]
        private float gravityConstant;
        [SerializeField]
        private float jumpStrength;

        public CharacterController CharacterController { get; private set; }

        void Awake()
        {
            CharacterController = GetComponent<CharacterController>();
        }

        public PlayerStateData GetNextFrameData(PlayerInputData input, PlayerStateData currentState)
        {
            bool w = input.Keyinputs[0];
            bool a = input.Keyinputs[1];
            bool s = input.Keyinputs[2];
            bool d = input.Keyinputs[3];
            bool space = input.Keyinputs[4];
            bool isRunning = input.Keyinputs[5];

            Vector3 eulerAngles = Quaternion.Euler(0.0f, input.Yaw, 0.0f).eulerAngles;

            gravity = new Vector3(0, currentState.Gravity, 0);

            Vector3 movement = Vector3.zero;

            if (w)
            {
                movement += Vector3.forward;
            }
            if (a)
            {
                movement += Vector3.left;
            }
            if (s)
            {
                movement += Vector3.back;
            }
            if (d)
            {
                movement += Vector3.right;
            }

            movement = Quaternion.Euler(0, eulerAngles.y, 0) * movement; // Move towards the look direction.
            movement.Normalize();
            movement *= (isRunning ? runSpeed : walkSpeed) * Time.fixedDeltaTime;
            movement += gravity * Time.fixedDeltaTime;

            if (CharacterController.isGrounded)
            {
                if (space)
                {
                    gravity = new Vector3(0, jumpStrength, 0);
                }
            }
            else
            {
                gravity -= new Vector3(0, gravityConstant, 0);
            }

            CharacterController.Move(movement);
            return new PlayerStateData(currentState.Id, gravity.y, transform.localPosition, input.Yaw, input.Pitch);
        }
    }
	
}
