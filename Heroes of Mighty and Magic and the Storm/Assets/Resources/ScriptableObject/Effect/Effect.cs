using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectTarget { origin, target }

public class Effect : ScriptableObject
{
    [HideInInspector]
    public Unit originUnit, targetUnit;
    [HideInInspector]
    public NodeItem targetNode;
    [HideInInspector]
    public int originPlayer;

    public EffectTarget target = EffectTarget.target;

    public AnimationClip fx;
    public Sound sound;

    public virtual void Init(Effect _parent)
    {
        originPlayer = _parent.originPlayer;
        originUnit = _parent.originUnit;
        targetUnit = _parent.targetUnit;
        targetNode = _parent.targetNode;
    }
    public virtual void Init(Unit _originUnit, Unit _targetUnit, NodeItem _targetNode)
    {
        originUnit = _originUnit;
        targetUnit = _targetUnit;
        targetNode = _targetNode;
    }
    public virtual void Init(Unit _originUnit)
    {
        originUnit = _originUnit;
    }

    public virtual void Invoke()
    {
        if (fx != null)
        {
            OneShotFXMgr.instance.Play(fx, targetUnit.transform.position);
        }

        if (sound != null)
        {
            SoundManager.instance.PlaySound(sound);
        }
    }

}
