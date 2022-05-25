using Mirror;
using UnityEngine;

/**
 * @author Victor Wikner
 *
 * Not implemented
 *
 * Supposed to change colour and set it so all players can see the difference
 */
public class ColourCustomization : NetworkBehaviour

    {

    //[SerializeField] private List<MeshRenderer> ObjectsToChangeColour = new List<MeshRenderer>();
    [SerializeField] private SkinnedMeshRenderer armorColour;
    //public event System.Action<Color32> OnPlayerColorChanged;

    //[SyncVar(hook = nameof(PlayerColorChanged))]
    [SyncVar]
    private Color32 color32;

    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    private byte red;
    private byte green;
    private byte blue;
    private void Awake()
    {
        red = (byte)PlayerPrefs.GetInt("redValue");
        green = (byte)PlayerPrefs.GetInt("greenValue");
        blue = (byte)PlayerPrefs.GetInt("blueValue");
        ChangeColour();
    }
    private void Start()
    {
        if (isLocalPlayer)
        {
            red = (byte)PlayerPrefs.GetInt("redValue");
            green = (byte)PlayerPrefs.GetInt("greenValue");
            blue = (byte)PlayerPrefs.GetInt("blueValue");
            CmdSetPlayerColour(red, green, blue);
        }
    }
    [Command]
    public void CmdSetPlayerColour(byte r, byte g, byte b)
    {
        red = r;
        green = g;
        blue = b;
        color32 = new Color32(r, g, b, 255);
        ChangeColourSimple();

    }

    private void ChangeColourSimple()
    {
        armorColour.material.SetColor(BaseColor, color32);
    }


    private void ChangeColour()
    {
        if (isLocalPlayer)
        {
            red = (byte)PlayerPrefs.GetInt("redValue");
            green = (byte)PlayerPrefs.GetInt("greenValue");
            blue = (byte)PlayerPrefs.GetInt("blueValue");
            color32 = new Color32(r: red, g: green, b: blue, a: 255);

            armorColour.material.SetColor(BaseColor, color32);
        }

    }
    /*void PlayerColorChanged(Color32 _, Color32 newPlayerColor)
    {
        OnPlayerColorChanged?.Invoke(newPlayerColor);
    }*/



    }

