using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSweet : MonoBehaviour {

    private GameSweet sweet;
    private IEnumerator moveCoroutine;//
    private void Awake()
    {
        sweet = GetComponent<GameSweet>();//获取当前的对象的脚本
    }
    public void Move(int newX,int newY,float time)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = MoveCoroutine(newX,newY,time);
        StartCoroutine(moveCoroutine);
      
    }
    public IEnumerator MoveCoroutine(int newX, int newY,float time)
    {
        sweet.X = newX;
        sweet.Y = newY;
        Vector3 startPos, endPos;
        startPos = transform.position;
        endPos= sweet.gameManager.CorrectPosition(sweet.X, sweet.Y);
        for (float t = 0; t < time; t+=Time.deltaTime)
        {
         
            sweet.transform.position = Vector3.Lerp(startPos, endPos, t / time);//第三个参数是起始位置到末位置的总路程百分比
            yield return 0;
        }
        sweet.transform.position = endPos;
    }
}
