using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSweet : MonoBehaviour {

    private int x;
    private int y;
    [HideInInspector]
    public GamaManager gameManager;//获取单例对象

    private MoveSweet moveComponent;
    public MoveSweet MoveComponent
    {
        get
        {
            return moveComponent;
        }
    }

    private GamaManager.SweetsType type;
    public GamaManager.SweetsType Type
    {
        get
        {
            return type;
        }
    }

    public int X
    {
        get
        {
            return x;
        }

        set
        {
            if(CanMove())
            x = value;
        }
    }

    public int Y
    {
        get
        {
            return y;
        }

        set
        {
            if(CanMove())
            y = value;
        }
    }

    public ColorSweet ColorComponent
    {
        get
        {
            return colorComponent;
        }
    }

    public ClearSweet ClearComponent
    {
        get
        {
            return clearComponent;
        }

    }

    
    private ColorSweet colorComponent;

    private ClearSweet clearComponent;
    
    //检测是否带有移动脚本来判断物体是否可以进行操作
    public bool CanMove()
    {
        return moveComponent != null;
    }
    public bool CanColor()
    {
        return colorComponent != null;//判断color组件是否存在
    }
    public bool CanClear()
    {
        return clearComponent != null;
    }
    /// <summary>
    /// 初始化产生的甜品位置信息
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <param name="_gameManager"></param>
    /// <param name="_type"></param>
    public void Init(int _x,int _y,GamaManager _gameManager,GamaManager.SweetsType _type)
    {
        X = _x;
        Y = _y;
        gameManager = _gameManager;
        type = _type;
    }
    private void Awake()
    {
        moveComponent = GetComponent<MoveSweet>();//获取脚本如果能获取 则说明可以移动
        colorComponent = GetComponent<ColorSweet>();
        clearComponent = GetComponent<ClearSweet>();
    }

    private void OnMouseEnter()
    {
        gameManager.EnterSweet(this);
    }
    private void OnMouseDown()
    {

        gameManager.PressSweet(this);
    }
    private void OnMouseUp()
    {
        gameManager.ReleaseSweet();
    }

  
}
