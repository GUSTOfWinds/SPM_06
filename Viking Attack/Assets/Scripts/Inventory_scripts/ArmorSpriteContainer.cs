using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmorSpriteContainer : MonoBehaviour
{
    [SerializeField] private Sprite[] armorSprites;

    public Sprite GetSprite(int index)
    {
        return armorSprites[index];
    }
}
