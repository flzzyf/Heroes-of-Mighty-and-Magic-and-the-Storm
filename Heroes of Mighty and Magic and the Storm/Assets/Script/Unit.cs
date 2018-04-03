using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public UnitType type;

    public SpriteRenderer sprite;
    public Animator animator;

    //朝向，-1为左，1为右
    int facing = 1;

    public int speed;

	void Start () 
    {
        if(type != null)
            InitUnitType();

	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            animator.Play("Attack");
        }


    }

    public void InitUnitType()
    {
        animator.runtimeAnimatorController = type.animControl;

        speed = type.speed;
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

    private void OnMouseEnter()
    {
        GameMaster.instance.ChangeMouseCursor(1);
    }

    private void OnMouseExit()
    {
        GameMaster.instance.ChangeMouseCursor();

    }

    private void OnMouseDown()
    {
        BattleManager.instance.ActionEnd();
    }


}
