using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MyData",menuName = "My Game/MyData")]
public class MyData : ScriptableObject
{
   //定义成员数据
   public string myString;
   public int myInt;
}
