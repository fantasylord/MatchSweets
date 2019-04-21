using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearLineSweet : ClearSweet
{

    public bool isRow;

    public override void Clear()
    {
        base.Clear();
        if (isRow)
        {
            sweet.gameManager.ClearRow(sweet.Y);
        }
        else
        {
            sweet.gameManager.ClearColumn(sweet.X);
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
