using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour {

    public UnitType type;

    public SpriteRenderer sprite;
    public Animator animator;
    public Text numUI;

    //朝向，-1为左，1为右
    int facing = 1;

    //单位属性
    [HideInInspector]
    public int speed, att, def, currentHP, num;
    [HideInInspector]
    public Vector2 damage;

    [HideInInspector]
    public Node node;

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
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            animator.SetBool("walking", true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            animator.SetBool("walking", false);
        }

    }

    public void InitUnitType()
    {
        animator.runtimeAnimatorController = type.animControl;

        speed = type.speed;
        att = type.att;
        def = type.def;
        damage = type.damage;
        currentHP = type.hp;
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

    public void ChangeNum(int _amount)
    {
        num = _amount;
        numUI.text = _amount.ToString();
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
