using UnityEngine;

public enum Layers { DeadUnit, Unit, ActionUnit}

public class SortingOrderMgr : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    float previousY;

	void Update ()
    {
        if(transform.position.y != previousY)
        {
            previousY = transform.position.y;
            spriteRenderer.sortingOrder = (int)transform.position.y * -1;
        }
    }

    public void SetSortingLayer(Layers _layer)
    {
        spriteRenderer.sortingLayerName = _layer.ToString();
    }
}
