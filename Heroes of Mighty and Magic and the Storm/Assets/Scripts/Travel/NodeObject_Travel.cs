using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TravelNodeType { empty, item, unit, hero, place, town }

public class NodeObject_Travel : NodeObject
{
    public TravelNodeType objectType;

    public AdvantureObject advantureObject;

    public SpriteRenderer spriteRenderer;

    public void Init()
    {
        spriteRenderer.sprite = advantureObject.icon;
    }
}
