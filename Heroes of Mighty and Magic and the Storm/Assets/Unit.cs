using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    int facing = 0;

    public SpriteRenderer sprite;

	void Start () {
		
	}
	
	void Update () 
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            FaceTarget(GameObject.Find("1"));
        }
	}

    void FaceTarget(GameObject _target)
    {
        print("qwe");

        int f = (_target.transform.position.x > transform.position.x) ? 0 : 1;

        if (f != facing)
        {
            sprite.flipX = !sprite.flipX;
            facing = f;
        }
    }
}
