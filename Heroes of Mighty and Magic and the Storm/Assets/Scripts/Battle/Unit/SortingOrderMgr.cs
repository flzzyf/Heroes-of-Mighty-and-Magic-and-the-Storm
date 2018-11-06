using UnityEngine;

public enum Layers { DeadUnit = -1, Unit = 0, ActionUnit = 1}

public class SortingOrderMgr : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    float previousY;

    int offset;

	void Update ()
    {
        if(transform.position.y != previousY)
        {
            previousY = transform.position.y;

            UpdateOrder();
        }
    }

    void UpdateOrder()
    {
        previousY = transform.position.y;
        spriteRenderer.sortingOrder = ((int)transform.position.y * -1 * 5) + offset;
    }

    public void SetSortingLayer(Layers _layer)
    {
        offset = (int)_layer;

        UpdateOrder();
    }
}
