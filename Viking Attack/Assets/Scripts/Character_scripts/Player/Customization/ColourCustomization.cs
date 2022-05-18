using System;
using Mirror;
using UnityEngine;

namespace Character_scripts.Player.Customization
{
    public class ColourCustomization : NetworkBehaviour

    {

    //[SerializeField] private List<MeshRenderer> ObjectsToChangeColour = new List<MeshRenderer>();
    [SerializeField] private SkinnedMeshRenderer armorColour;
    public event System.Action<Color32> OnPlayerColorChanged;

    [SyncVar(hook = nameof(PlayerColorChanged))]
    private Color32 color32;

    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        ChangeColour();
    }

    private void ChangeColour()
    {

        byte red = (byte)PlayerPrefs.GetInt("redValue");
        byte green = (byte)PlayerPrefs.GetInt("greenValue");
        byte blue = (byte)PlayerPrefs.GetInt("blueValue");
        color32 = new Color32(r: red, g: green, b: blue, a: 255);
        
        armorColour.material.SetColor(BaseColor, color32);

    }
    void PlayerColorChanged(Color32 _, Color32 newPlayerColor)
    {
        OnPlayerColorChanged?.Invoke(newPlayerColor);
    }



    }
}
