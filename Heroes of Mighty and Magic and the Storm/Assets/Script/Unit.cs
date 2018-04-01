using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public UnitType type;

    public SpriteRenderer sprite;
    public Animator animator;

    //朝向，-1为左，1为右
    int facing = 1;
    int previousFacing = 1;

	void Start () 
    {
        if(type != null)
            Init();

	}

    void Init()
    {
        animator.runtimeAnimatorController = type.animControl;
    }

    public void FaceTarget(GameObject _target)
    {
        int targetInTheRight = (_target.transform.position.x > transform.position.x) ? 1 : -1;

        //previousFacing = targetInTheRight;

        if (targetInTheRight != facing)
        {
            sprite.flipX = !sprite.flipX;
            facing = targetInTheRight;
        }
    }

}
