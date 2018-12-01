using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneShotFXMgr : Singleton<OneShotFXMgr>
{
    public GameObject prefab_fx;

    public Vector3 offset;

    public void Play(AnimationClip _anim, Vector3 _pos)
    {
        GameObject go = Instantiate(prefab_fx, _pos + offset, Quaternion.identity);

        go.GetComponent<Animator>().Play(_anim.name);
    }

    public void Play(GameObject _prefab, Vector3 _pos)
    {
        GameObject go = Instantiate(_prefab, _pos + offset, Quaternion.identity);

        Destroy(go, 1.5f);
    }
}
