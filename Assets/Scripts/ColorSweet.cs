using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSweet : MonoBehaviour
{

    public Dictionary<ColorEnum, Sprite> colorSpritDict;
    public enum ColorEnum
    {
        YELLOW,
        PURPLE,
        RED,
        BLUE,
        GREEN,
        PINK,
        ANYCOLORS,
        COUNT
      
    }
    [System.Serializable]
    public struct ColorSprite
    {
        public ColorEnum color;
        public Sprite sprite;
    }
    public ColorSprite[] colorSprites;
    private ColorEnum color;
    private SpriteRenderer spriteRend;//获取甜品预制体渲染器
    [HideInInspector]
    public int NumColors
    {
        get { return colorSprites.Length; }
    }

    public ColorEnum Color
    {
        get
        {
            return color;
        }

        set
        {
            color = value;
        }
    }
    public void SetColor(ColorEnum _color)
    {
        color = _color;
        if (colorSpritDict.ContainsKey(color))
        {
            spriteRend.sprite = colorSpritDict[color];
        }
    }
    private void Awake()
    {
        spriteRend = transform.Find("Sweet").GetComponent<SpriteRenderer>();
        colorSpritDict = new Dictionary<ColorEnum, Sprite>();
        for (int i = 0; i < colorSprites.Length; i++)
        {
            if (!colorSpritDict.ContainsKey(colorSprites[i].color))
            {
                colorSpritDict.Add(colorSprites[i].color, colorSprites[i].sprite);
            }
        }
    }
}
