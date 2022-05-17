using UnityEngine;

namespace Character_scripts.Player.Customization
{
    public class ColourCustomization : MonoBehaviour

    {

    //[SerializeField] private List<MeshRenderer> ObjectsToChangeColour = new List<MeshRenderer>();
    [SerializeField] private SkinnedMeshRenderer armorColour;

    private Color32 color32;

    public  void Start()
    {
        ChangeColour();
    }

    private void ChangeColour()
    {

        byte red = (byte)PlayerPrefs.GetInt("redValue");
        byte green = (byte)PlayerPrefs.GetInt("greenValue");
        byte blue = (byte)PlayerPrefs.GetInt("blueValue");
        color32 = new Color32(r: red, g: green, b: blue, a: 255);
        
        armorColour.material.SetColor("_BaseColor", color32);

    }



    }
}
