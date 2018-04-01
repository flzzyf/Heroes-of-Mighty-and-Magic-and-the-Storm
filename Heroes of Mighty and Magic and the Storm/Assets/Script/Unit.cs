using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public UnitType type;

    public SpriteRenderer sprite;
    public Animator animator;

    //朝向，-1为左，1为右
    int facing = 1;

	void Start () 
    {
        if(type != null)
            InitUnitType();

	}

    public void InitUnitType()
    {
        animator.runtimeAnimatorController = type.animControl;
    }

    public void Flip()
    {
        sprite.flipX = !sprite.flipX;
        facing *= -1;
    }

    public bool FaceTarget(GameObject _target)
    {
        int targetInTheRight = (_target.transform.position.x > transform.position.x) ? 1 : -1;

        if (targetInTheRight != facing)
        {
            Flip();

            return true;    //转向了
        }
        return false;
    }

}
