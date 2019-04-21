using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearColorSweet : ClearSweet {
    private ColorSweet.ColorEnum clearColor;
    
    public ColorSweet.ColorEnum ClearColor
    {
        get
        {
            return clearColor;
        }

        set
        {
            clearColor = value;
        }
    }
    public override void Clear()
    {
        base.Clear();
        sweet.gameManager.ClearAnyColors(clearColor);
    }
}
