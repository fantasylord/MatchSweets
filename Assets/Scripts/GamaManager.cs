using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamaManager : MonoBehaviour
{
    private static GamaManager _instance;
    public int xColumns;
    public int yRows;
    public GameObject grid;
    private GameSweet pressedSweet, enteredSweet;
    private bool isGameOver;

    public Dictionary<SweetsType, GameSweet> sweetPrefabDict;//存放预制体 和对应键值
    public SweetPerfab[] sweetPerfabs;//所需游戏块预制体
    public GameSweet[,] sweetsarray;
    public Text timeText;//游戏时间内容
    public Text scoreText;
    public Text finalScore;
    private float addScoreTime;
    private float currentScore;
    public GameObject gameOverPanel;
    public int playscore;
    public float timeNow=60;
    public float fillTime = 0.1f;
    public static GamaManager Instance
    {
        get
        {
            return _instance;
        }

        set
        {
            _instance = value;
        }
    }

    public enum SweetsType
    {
        EMPTY,
        NORMAL,
        BARRIER,
        ROW_CLEAR,
        COLUMN_CLEAR,
        RAINBOWCANDY,
        COUNT
    }


    [System.Serializable]
    public struct SweetPerfab
    {
        public SweetsType type;
        public GameSweet prefab;
    }
    void Start()
    {
        sweetPrefabDict = new Dictionary<SweetsType, GameSweet>();
        for (int i = 0; i < sweetPerfabs.Length; i++)//加载sweet种类
        {
            if (!sweetPrefabDict.ContainsKey(sweetPerfabs[i].type))
                sweetPrefabDict.Add(sweetPerfabs[i].type, sweetPerfabs[i].prefab);
        }
        for (int i = 0; i < xColumns; i++)//产生背景
        {
            for (int j = 0; j < yRows; j++)
            {
                //  print("x" + transform.position.x + "y" + transform.position.y);
                Instantiate(grid, CorrectPosition(new Vector3(i, j, 0)), Quaternion.identity).transform.SetParent(transform);
            }
        }
        sweetsarray = new GameSweet[yRows, xColumns];
        for (int i = 0; i < xColumns; i++)//产生sweets空块
        {
            for (int j = 0; j < yRows; j++)
            {

                CreateSweet(i, j, SweetsType.EMPTY);
                print("x" + transform.position.x + "y" + transform.position.y);
                //sweetsarray[j,i]= Instantiate(sweetPrefabDict[SweetsType.NORMAL], CorrectPosition(new Vector3(i, j, 0)), Quaternion.identity);
                //sweetsarray[j,i].transform.SetParent(transform);
                //// Instantiate( new Vector3(transform.position.x - xColumns / 2.0f + i, transform.position.y + yRows / 2.0f - j, 0), Quaternion.identity).transform.SetParent(transform);
                //if (sweetsarray[j, i].CanColor())
                //{
                //    sweetsarray[j, i].ColorComponent.SetColor((ColorSweet.ColorEnum)Random.Range(0,sweetsarray[j,i].ColorComponent.NumColors));
                //}
            }
        }

        int random1_x= Random.Range(4, xColumns)
            , random1_y = Random.Range(4, yRows)
            , random2_x= Random.Range(0, xColumns)
            , random2_y= Random.Range(0, yRows);//BARRIER 
        Destroy(sweetsarray[random1_y, random1_x].gameObject);
        Destroy(sweetsarray[random2_y, random2_x].gameObject);
        CreateSweet(random1_x, random1_y, SweetsType.BARRIER);//产生障碍
            CreateSweet(random2_x, random2_y, SweetsType.BARRIER);//产生障碍

       

        StartCoroutine(AllFill());
    }
    public Vector3 CorrectPosition(Vector3 v3)
    {
        v3.x += transform.position.x - xColumns / 2.0f + 0.5f;
        v3.y = transform.position.y + yRows / 2.0f - 0.5f - v3.y;
        return v3;
    }
    public Vector3 CorrectPosition(int x, int y)
    {
        return new Vector3(transform.position.x - xColumns / 2.0f + 0.5f + x, transform.position.y + yRows / 2.0f - 0.5f - y, 0);
    }
    private void Awake()
    {
        Instance = this;

    }

    void Update()
    {
        if (isGameOver)
        {
            return;
        }
        timeNow -= Time.deltaTime;
        if (timeNow <= 0)//游戏结束
        {
            timeNow = 0;
            finalScore.text = scoreText.text = playscore.ToString();
            timeText.enabled=false;
            gameOverPanel.SetActive(true);
            isGameOver = true;
            return;
        }
        timeText.text = timeNow.ToString("0");
        if(addScoreTime<=0.05f)
        {
            addScoreTime += Time.deltaTime;
        }
        else
        {
            if(currentScore<playscore)
            {
                currentScore++;
                scoreText.text = currentScore.ToString();
            }
        }
    
    }
    public GameSweet CreateSweet(int x, int y, SweetsType type)
    {
        print(x + "x,y" + y);
        GameSweet go = Instantiate(sweetPrefabDict[type], CorrectPosition(x, y), Quaternion.identity);
        go.transform.parent = transform;
        sweetsarray[y, x] = go;
        sweetsarray[y, x].Init(x, y, Instance, type);
        return sweetsarray[y, x];
    }
    public IEnumerator AllFill()//协程
    {
        bool needRefill = true;
        while (needRefill)
        {
            yield return new WaitForSeconds(fillTime*3);
            while (Fill())
            {
                yield return new WaitForSeconds(fillTime);
            }
            needRefill = ClearAllMatchedSweet();
        }

    }

    public bool Fill()
    {
        bool filledNotFinshed = false;

        for (int i = 0; i < xColumns; i++)  //最顶层单独填充因此排除,填充方向为Y。
        {
            for (int j = yRows - 2; j >= 0; j--)
            {

                GameSweet sweet = sweetsarray[j, i];

                if (sweet.CanMove())
                {
                    //  print(j + "," + i + 1);
                    GameSweet sweetfill = sweetsarray[j + 1, i];//由于排列时 从顶到底  j+1为下一层 而不是上一层
                    if (sweetfill.Type == SweetsType.EMPTY)//垂直填充,下方为空块
                    {
                        Destroy(sweetfill.gameObject);
                        sweet.MoveComponent.Move(i, j + 1, fillTime);
                        sweetsarray[j + 1, i] = sweet;
                        CreateSweet(i, j, SweetsType.EMPTY);
                        filledNotFinshed = true;
                        //  print("基本填充" + i + "," + j);
                    }
                    else //下方有障碍时 填充方法 为斜下 左或右
                    {
                        for (int down = -1; down <= 1; down++)//优先斜左下方滑动
                        {
                            if (down != 0)
                            {
                                int downx = i + down;
                                if (downx >= 0 && downx < xColumns)
                                {
                                    GameSweet downSweet = sweetsarray[j + 1, downx];
                                    if (downSweet.Type == SweetsType.EMPTY)
                                    {
                                        bool canfill = true;
                                        for (int moveY = j; moveY >= 0; moveY--)
                                        {
                                            GameSweet sweetmoveY = sweetsarray[moveY, downx];
                                            if (sweetmoveY.CanMove())
                                            {
                                                break;
                                            }
                                            else if (!sweetmoveY.CanMove() && sweetmoveY.Type != SweetsType.EMPTY)//斜向填充条件
                                            {
                                                canfill = false;
                                                break;
                                            }
                                        }
                                        if (!canfill)
                                        {
                                            Destroy(downSweet.gameObject);
                                            sweet.MoveComponent.Move(downx, j + 1, fillTime);
                                            sweetsarray[j + 1, downx] = sweet;
                                            CreateSweet(i, j, SweetsType.EMPTY);
                                            filledNotFinshed = true;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }

            }
        }
        for (int i = 0; i < xColumns; i++)//最顶层
        {
            GameSweet sweet = sweetsarray[0, i];
            if (sweet.Type == SweetsType.EMPTY)//生成sweet
            {
                GameSweet go = Instantiate(sweetPrefabDict[SweetsType.NORMAL], CorrectPosition(i, -1), Quaternion.identity);
                go.transform.SetParent(transform);
                sweetsarray[0, i] = go.GetComponent<GameSweet>();
                sweetsarray[0, i].Init(i, -1, Instance, SweetsType.NORMAL);
                sweetsarray[0, i].MoveComponent.Move(i, 0, fillTime);
                sweetsarray[0, i].ColorComponent.SetColor((ColorSweet.ColorEnum)(Random.Range(0, sweetsarray[0, i].ColorComponent.NumColors)));
                filledNotFinshed = true;
                // print("顶层填充" + i + "," + yRows + ",," + xColumns);

            }
        }

        return filledNotFinshed;
    }

    /// <summary>
    /// 是否相邻
    /// </summary>
    /// <param name="sweetOnclick1">第一个单击块</param>
    /// <param name="sweetOnclick2">第二个单机块</param>
    /// <returns></returns>
    public bool IsNeighbor(GameSweet sweetOnclick1, GameSweet sweetOnclick2)
    {

        return (sweetOnclick1.X == sweetOnclick2.X && Mathf.Abs(sweetOnclick1.Y - sweetOnclick2.Y) == 1) || (sweetOnclick1.Y == sweetOnclick2.Y && Mathf.Abs(sweetOnclick1.X - sweetOnclick2.X) == 1);


    }
    //鼠标操作交换位置时
    public void ExChangeSweets(GameSweet sweetOnclick1, GameSweet sweetOnclick2)
    {

        if (sweetOnclick1.Equals(sweetOnclick2))
            return; 
        if (sweetOnclick1.CanMove() && sweetOnclick2.CanMove())
        {
            sweetsarray[sweetOnclick1.Y, sweetOnclick1.X] = sweetOnclick2;
            sweetsarray[sweetOnclick2.Y, sweetOnclick2.X] = sweetOnclick1;//记录位子更新

            if (MatchSweets(sweetOnclick1, sweetOnclick2.X, sweetOnclick2.Y) != null ||
                MatchSweets(sweetOnclick2, sweetOnclick1.X, sweetOnclick1.Y) != null ||
                sweetOnclick1.Type==SweetsType.RAINBOWCANDY||
                sweetOnclick2.Type == SweetsType.RAINBOWCANDY)//可以交换时
            {
                sweetsarray[sweetOnclick1.Y, sweetOnclick1.X] = sweetOnclick2;
                sweetsarray[sweetOnclick2.Y, sweetOnclick2.X] = sweetOnclick1;//记录位子更新

                int tempx = sweetOnclick1.X
                    , tempy  = sweetOnclick1.Y;
                sweetOnclick1.MoveComponent.Move(sweetOnclick2.X, sweetOnclick2.Y, fillTime);
                sweetOnclick2.MoveComponent.Move(tempx, tempy, fillTime);

                if (sweetOnclick1.Type==SweetsType.RAINBOWCANDY&&sweetOnclick1.CanClear()&&sweetOnclick2.CanClear())
                {
                    ClearColorSweet clearcolor = sweetOnclick1.GetComponent<ClearColorSweet>();
                    if (clearcolor != null)
                    {
                        clearcolor.ClearColor = sweetOnclick2.ColorComponent.Color;
                    }
                    ClearSweet(sweetOnclick1.X, sweetOnclick1.Y);
                }

                if (sweetOnclick2.Type == SweetsType.RAINBOWCANDY && sweetOnclick1.CanClear() && sweetOnclick2.CanClear())
                {
                    ClearColorSweet clearcolor = sweetOnclick2.GetComponent<ClearColorSweet>();
                    if (clearcolor != null)
                    {
                        clearcolor.ClearColor = sweetOnclick1.ColorComponent.Color;
                    }
                    ClearSweet(sweetOnclick2.X, sweetOnclick2.Y);

                }
                pressedSweet = null;
                enteredSweet = null;
                ClearAllMatchedSweet();
                StartCoroutine(AllFill());
            }
            else
            {
                sweetsarray[sweetOnclick1.Y, sweetOnclick1.X] = sweetOnclick1;
                sweetsarray[sweetOnclick2.Y, sweetOnclick2.X] = sweetOnclick2;//记录位子更新

            }

        }
    }

    #region 鼠标操作事件
    public void PressSweet(GameSweet _pressedSweet)
    {
        pressedSweet = _pressedSweet;
    }
    public void EnterSweet(GameSweet _enteredSweet)
    {
        enteredSweet = _enteredSweet;
    }
    public void ReleaseSweet()
    {
        if (IsNeighbor(pressedSweet, enteredSweet))
        {
            ExChangeSweets(pressedSweet, enteredSweet);
        }
    }
    #endregion

    /// <summary>
    /// 按列获取消除对象
    /// </summary>
    /// <returns></returns>
    public List<List<GameSweet>> GetMatchColumnSweet()
    {
        List<List<GameSweet>> sweetClearList = new List<List<GameSweet>>();
        for (int x = 0; x < xColumns; x++)
        {
            foreach (ColorSweet.ColorEnum item in System.Enum.GetValues(typeof(ColorSweet.ColorEnum)))
            {
                List<GameSweet> sweetList = new List<GameSweet>();
                if (sweetsarray[0, x].CanColor() &&
                    sweetsarray[0, x].ColorComponent.Color == item)
                    sweetList.Add(sweetsarray[0, x]);
                for (int y = yRows + 1; y > 0; y--)
                {
                    if (sweetsarray[y, x].CanColor() &&
                       sweetsarray[y, x].ColorComponent.Color == item &&
                       sweetsarray[y - 1, x].ColorComponent.Color == sweetsarray[y, x].ColorComponent.Color)
                        sweetList.Add(sweetsarray[y, x]);
                }
                if (sweetList.Count >= 3)
                {
                    sweetClearList.Add(sweetList);
                }
                else
                    sweetList.Clear();
            }
        }
        return sweetClearList;
    }

    /// <summary>
    /// 按行获取消除对象
    /// </summary>
    /// <returns></returns>
    public List<List<GameSweet>> GetMatchRowSweet()
    {

        List<List<GameSweet>> sweetClearList = new List<List<GameSweet>>();
        for (int y = 0; y < yRows; y++)//行遍历消除
        {
            foreach (ColorSweet.ColorEnum item in System.Enum.GetValues(typeof(ColorSweet.ColorEnum)))
            {
                List<GameSweet> sweetList = new List<GameSweet>();
                if (sweetsarray[y, 0].CanColor() &&
                        sweetsarray[y, 0].ColorComponent.Color == item)
                    sweetList.Add(sweetsarray[y, 0]);
                for (int x = 1; x < xColumns; x++)
                {
                    if (sweetsarray[y, x].CanColor() &&
                        sweetsarray[y, x].ColorComponent.Color == item &&
                        sweetsarray[y, x - 1].ColorComponent.Color == sweetsarray[y, x].ColorComponent.Color)
                        sweetList.Add(sweetsarray[y, x]);
                }

                if (sweetList.Count >= 3)
                {
                    sweetClearList.Add(sweetList);
                }
                else
                    sweetList.Clear();
            }

        }
        return sweetClearList;
    }

    /// <summary>
    /// 获取指定匹配消除对象
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public List<GameSweet> GetMatchAllSweet(GameObject color)
    {
        List<GameSweet> list = new List<GameSweet>();
        return list;
    }

    public List<GameSweet> MatchSweets(GameSweet sweet, int newX, int newY)
    {
        if (sweet.CanColor())
        {
            ColorSweet.ColorEnum color = sweet.ColorComponent.Color;
            List<GameSweet> matchRowSweets = new List<GameSweet>();
            List<GameSweet> matchLineSweets = new List<GameSweet>();
            List<GameSweet> finishedMatchweets = new List<GameSweet>();

            //----行匹配
            matchRowSweets.Add(sweet);
            for (int i = 0; i <= 1; i++)//0，1代表方向
            {
                for (int xDistance = 1; xDistance < xColumns; xDistance++)
                {
                    int x;
                    if (i == 0)
                    {
                        x = newX - xDistance;
                    }
                    else
                    {
                        x = newX + xDistance;
                    }
                    if (x < 0 || x >= xColumns)
                    {
                        break;
                    }
                    if (sweetsarray[newY, x].CanColor() &&
                        sweetsarray[newY, x].ColorComponent.Color == color)
                    { matchRowSweets.Add(sweetsarray[newY, x]); }
                    else
                    {
                        break;
                    }
                }
            }
            if (matchRowSweets.Count >= 3)
            {
                for (int i = 0; i < matchRowSweets.Count; i++)
                {
                    finishedMatchweets.Add(matchRowSweets[i]);
                }
                for (int i = 0; i < matchRowSweets.Count; i++)
                {
                    for (int j = 0; j <= 1; j++)//方向
                    {
                        for (int yDisTance = 1; yDisTance < yRows; yDisTance++)
                        {
                            int y;
                            if (j == 0)
                            {
                                y = newY - yDisTance;
                            }
                            else
                            {
                                y = newY + yDisTance;
                            }
                            if (y < 0 || y >= yRows)
                            {
                                break;
                            }
                            if (sweetsarray[y, matchRowSweets[i].X].CanColor()
                                && sweetsarray[y, matchRowSweets[i].X].ColorComponent.Color == color)
                            {
                                matchLineSweets.Add(sweetsarray[y, matchRowSweets[i].X]);
                            }
                            else
                            {
                                break;//找到第一个不同颜色时直接跳出
                            }
                        }
                    }
                    if (matchLineSweets.Count < 2)//该列表未包含基准颜色块
                    {
                        matchLineSweets.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < matchLineSweets.Count; j++)
                        {
                            finishedMatchweets.Add(matchLineSweets[j]);
                        }
                        break;
                    }
                    //finishedMatchweets.Add(matchRowSweets[i]);
                }
            }
            if (finishedMatchweets.Count >= 3)
            { print("匹配"); return finishedMatchweets; }

          
            matchRowSweets.Clear();
            matchLineSweets.Clear();

            matchLineSweets.Add(sweet);
            for (int i = 0; i <= 1; i++)//0，1代表方向
            {
                for (int yDistance = 1; yDistance < yRows; yDistance++)
                {
                    int y;
                    if (i == 0)
                    {
                        y = newY - yDistance;
                    }
                    else
                    {
                        y = newY + yDistance;
                    }
                    if (y < 0 || y >= yRows)
                    {
                        break;
                    }
                    if (sweetsarray[y, newX].CanColor() &&
                        sweetsarray[y, newX].ColorComponent.Color == color)
                    { matchLineSweets.Add(sweetsarray[y, newX]); }
                    else
                    {
                        break;
                    }
                }
            }
            if (matchLineSweets.Count >= 3)
            {
                for (int i = 0; i < matchLineSweets.Count; i++)
                {
                    finishedMatchweets.Add(matchLineSweets[i]);
                }


                for (int i = 0; i < matchLineSweets.Count; i++)
                {
                    for (int j = 0; j <= 1; j++)//方向
                    {
                        for (int xDisTance = 1; xDisTance < xColumns; xDisTance++)
                        {
                            int x;
                            if (j == 0)
                            {
                                x = newX - xDisTance;
                            }
                            else
                            {
                                x = newX + xDisTance;
                            }
                            if (x < 0 || x >= xColumns)
                            {
                                break;
                            }
                            if (sweetsarray[matchLineSweets[i].Y, x].CanColor()
                                && sweetsarray[matchLineSweets[i].Y, x].ColorComponent.Color == color)
                            {
                                matchRowSweets.Add(sweetsarray[matchLineSweets[i].Y, x]);
                            }
                            else
                            {
                                break;//找到第一个不同颜色时直接跳出
                            }
                        }
                    }
                    if (matchRowSweets.Count < 2)//该列表未包含基准颜色块
                    {
                        matchRowSweets.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < matchRowSweets.Count; j++)
                        {
                            finishedMatchweets.Add(matchRowSweets[j]);
                        }
                        break;
                    }
                    //finishedMatchweets.Add(matchLineSweets[i]);
                }
            }
            if (finishedMatchweets.Count >= 3)
            { print("匹配"); return finishedMatchweets; }
        }

        print("匹配失败");
        return null;
    }

    public bool ClearSweet(int x, int y)
    {
        print("清除块");

        if (sweetsarray[y, x].CanClear() && !sweetsarray[y, x].ClearComponent.IsClearing)
        {
            sweetsarray[y, x].ClearComponent.Clear();
            ClearCookie(x,y);
            CreateSweet(x, y, SweetsType.EMPTY);
            return true;
        }
        return false;
    }

    private bool ClearAllMatchedSweet()//清除所有块方法
    {
        if (isGameOver)
            return false;
        print("清除所有块方法");
        bool needRefill = false;
        for (int y = 0; y < yRows; y++)
        {
            for (int x = 0; x < xColumns; x++)
            {
                if (sweetsarray[y, x].CanClear())
                {
                    List<GameSweet> matchlist = MatchSweets(sweetsarray[y, x], x, y);
                    if (matchlist != null)
                    {
                        SweetsType specialSweetsType = SweetsType.COUNT;//特殊类甜品产生flag;
                        GameSweet randomSweet = matchlist[Random.Range(0, matchlist.Count)];
                        int specialSweetX = randomSweet.X;
                        int specialSweetY = randomSweet.Y;
                        if(matchlist.Count==4)
                        {
                            specialSweetsType=(SweetsType)Random.Range((int)SweetsType.ROW_CLEAR, (int)SweetsType.COLUMN_CLEAR);
                        }
                        //5个同样块消除时判定是行还是列  RainbowSweet产生条件
                        else if (matchlist.Count > 4)
                        {
                            specialSweetsType = SweetsType.RAINBOWCANDY;
                        }
                        for (int i = 0; i < matchlist.Count; i++)//---清除符合条件的所有块
                        {
                            if (ClearSweet(matchlist[i].X, matchlist[i].Y))
                            {
                                needRefill = true;
                            }
                        }
                
                        if (specialSweetsType != SweetsType.COUNT)
                        {
                            Destroy(sweetsarray[specialSweetY, specialSweetX]);
                            GameSweet newsweet = CreateSweet(specialSweetX, specialSweetY, specialSweetsType);
                            if (newsweet.CanColor()&& specialSweetsType == SweetsType.ROW_CLEAR || specialSweetsType == SweetsType.COLUMN_CLEAR)
                            {
                                newsweet.ColorComponent.SetColor(matchlist[0].ColorComponent.Color);
                            }
                            else if (specialSweetsType == SweetsType.RAINBOWCANDY && newsweet.CanColor())
                            {
                                newsweet.ColorComponent.SetColor(ColorSweet.ColorEnum.ANYCOLORS);
                             
                            }
                        }

                    }
                }
            }
        }
        return needRefill;
    }

    private void ClearCookie(int x,int y)//障碍物清除
    {
        for (int neighborx = x-1; neighborx <= x+1; neighborx++)
        {
            if (neighborx != x&& neighborx>=0&&neighborx<xColumns)
            {
                if (sweetsarray[y,neighborx].Type==SweetsType.BARRIER&& sweetsarray[y, neighborx].CanClear())
                {
                    sweetsarray[y, neighborx].ClearComponent.Clear();
                    CreateSweet(neighborx, y, SweetsType.EMPTY);
                }
            }
        }

        for (int neighbory = y - 1; neighbory <= y + 1; neighbory++)
        {
            if (neighbory != y && neighbory >= 0 && neighbory < yRows)
            {
                if (sweetsarray[neighbory, x].Type == SweetsType.BARRIER && sweetsarray[neighbory, x].CanClear())
                {
                    sweetsarray[neighbory, x].ClearComponent.Clear();
                    CreateSweet(x, neighbory, SweetsType.EMPTY);
                }
            }
        }
    }

    public void ReturnToMain()
    {
        SceneManager.LoadScene(0);

    }
    public void Replay()
    {
        SceneManager.LoadScene(1);
    }

    //清除行
    public void ClearRow(int row)
    {
        for(int x = 0; x < xColumns; x++)
        {
            ClearSweet(x, row);
        }
    }
    //清除列
    public void ClearColumn(int colum)
    {
        for(int y = 0; y < yRows; y++)
        {
            ClearSweet(colum, y);
        }
    }
    //清除单一颜色
    public void ClearAnyColors(ColorSweet.ColorEnum color)
    {
        print("清除单一颜色");
        for (int x = 0; x < xColumns; x++)
        {
            for (int y = 0; y < yRows; y++)
            {
                 if(sweetsarray[y,x].CanColor()&&(sweetsarray[y,x].ColorComponent.Color==color||color==ColorSweet.ColorEnum.ANYCOLORS))
                {
                    ClearSweet(x, y);
                }
            }
        }
    }
  
}
