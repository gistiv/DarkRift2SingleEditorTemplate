using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Client.Gameplay
{
	
	public class BulletParticle : MonoBehaviour
	{
		private new ParticleSystem particleSystem;

	    void Start()
	    {
			particleSystem = GetComponent<ParticleSystem>();
	    }

        private void OnParticleCollision(GameObject other)
        {
            if (other.CompareTag(nameof(Tags.Player)))
            {
				return;
            }

			List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
			int events = particleSystem.GetCollisionEvents(other, collisionEvents);

            foreach (ParticleCollisionEvent collision in collisionEvents)
            {
				GameObject go = Instantiate(Resources.Load<GameObject>(@"Prefabs\Gameplay\EnvironmentHit"), collision.intersection, Quaternion.LookRotation(collision.normal), other.transform);
				Destroy(go, 2f);
            }
        }
    }
	
}
