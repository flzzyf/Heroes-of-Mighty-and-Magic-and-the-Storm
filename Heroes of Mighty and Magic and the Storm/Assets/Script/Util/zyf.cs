using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zyf
{
    static string[] type = { "【提示】" , "【警告】"};

    static string[] color = { "green", "red" };

    public static void Out(string _msg, int _type = 0)
    {
        Debug.Log("<color=" + color[_type] + ">" + type[_type] + _msg + "</color>");
    }
}
