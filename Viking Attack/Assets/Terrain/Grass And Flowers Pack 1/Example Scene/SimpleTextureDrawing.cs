using UnityEngine;

namespace Terrain.Grass_And_Flowers_Pack_1.Example_Scene
{
	public class SimpleTextureDrawing : MonoBehaviour 
	{
		public Material aMaterial;
		public RenderTexture[] aTextures;

		void OnGUI()
		{
			Graphics.DrawTexture(new Rect(Screen.width - 300, -2, 300, 300), aTextures[0], aMaterial);
			Graphics.DrawTexture(new Rect(Screen.width - 300, 302, 300, 300), aTextures[1], aMaterial);
		}
	}
}

