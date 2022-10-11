using UnityEngine;

namespace Client
{

    public class Crosshair : MonoBehaviour
    {
        private void OnGUI()
        {
            Texture2D texture2D = new Texture2D(1, 1);
            texture2D.SetPixel(0, 0, Color.white);
            texture2D.wrapMode = TextureWrapMode.Repeat;
            texture2D.Apply();
            GUI.DrawTexture(new Rect(Screen.width / 2, (Screen.height / 2) - 4, 2, 10), texture2D);
            GUI.DrawTexture(new Rect((Screen.width / 2) - 4, Screen.height / 2, 10, 2), texture2D);
        }

    }
}
