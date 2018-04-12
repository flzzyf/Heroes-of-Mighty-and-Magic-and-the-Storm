using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zyf
{
    public enum Type {tip, warning};

    static string[] type = { "【提示】" , "【警告】"};

    static string[] color = { "green", "red" };

    public static void Out(string _msg, Type _type = Type.tip)
    {
        Debug.Log("<color=" + color[(int)_type] + ">" + type[(int)_type] + _msg + "</color>");
    }
}
