using System.Collections.Generic;
using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    public Sprite icon;

    void Start()
    {
        if (icon != null)
        {
            GameObject go = Instantiate(MinimapMgr.instance.prefab_iconObject, transform);
            go.GetComponent<SpriteRenderer>().sprite = icon;
        }
    }
}
