using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeUnit : MonoBehaviour {

    public SpriteRenderer bg;

    private void OnMouseEnter()
    {
        bg.enabled = true;
    }

    private void OnMouseExit()
    {
        bg.enabled = false;
    }
}
