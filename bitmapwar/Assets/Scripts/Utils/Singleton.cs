using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : System.Object where T : Singleton<T>, new()
{
    private static T _inst;

    public static T Inst
    {
        get
        {
            if(_inst == null)
            {
                _inst = new T();
            }
            return _inst;
        }
    }
    
}
