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
    public GameObject nodeUnit;

    Dictionary<string, string> animName = new Dictionary<string, string>{
        {"move", "walking"}, {"attack", "Attack"}
    };


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

    public void PlayAnimation(string _anim, int _value = -1)
    {
        if (_value != -1)
        {
            animator.SetBool(animName[_anim], (_value == 1) ? true : false);

        }
        else
        {
            animator.Play(animName[_anim]);
        }
    }

    void TakeDamage(int _amount)
    {
        int a = _amount / type.hp;

        print(a);
    }

    float DamageRate(int _att, int _def)    //攻防伤害倍率计算
    {
        float r = 1;
        if (_att > _def)
            r = (1 + (_att - _def) * 0.05f);
        else if (_att < _def)
            r = (1 - (_def - _att) * 0.025f);

        return r;
    }

}
