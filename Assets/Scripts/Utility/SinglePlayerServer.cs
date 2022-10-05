using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utility
{
	
	public class SinglePlayerServer : MonoBehaviour
	{
	    // Start is called before the first frame update
	    void Awake()
	    {
            if (Application.isEditor)
            {
				Destroy(Camera.main.gameObject);
				SceneManager.LoadScene("Login", LoadSceneMode.Additive);
			}
            else
            {
				Destroy(this);
            }
	    }
	}
	
}
