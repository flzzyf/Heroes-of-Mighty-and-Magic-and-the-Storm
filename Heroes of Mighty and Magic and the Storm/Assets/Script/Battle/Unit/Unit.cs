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

    [HideInInspector]
    public int player;

	void Start () 
    {
        if(type != null)
            InitUnitType();

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

    public void ChangeNum(int _amount)
    {
        num = _amount;
        numUI.text = _amount.ToString();
    }

    public void ChangeNum(int _amount, int _sign)
    {
        int n = num + _amount * _sign;

        ChangeNum(n);
    }

    public void ChangeHp(int _amount)
    {
        currentHP = _amount;
    }

    public void ChangeHp(int _amount, int _sign)
    {
        int n = currentHP + _amount * _sign;

        ChangeHp(n);
    }

    public void TakeDamage(int _amount)
    {
        if(_amount > (num - 1) * type.hp + currentHP)
        {
            //死了
            Death();

            return;
        }

        if(_amount < currentHP)
        {
            ChangeHp(_amount, -1);
        }
        else
        {
            int deathNum = 1 + _amount / type.hp;
            ChangeNum(deathNum, -1);
            int remainHp = type.hp - (_amount - currentHP);
            ChangeHp(remainHp);
        }

        print(currentHP);
    }

    void Death()
    {
        gameObject.SetActive(false); 
    }

    public void ChangeOutline(float _value = 0)
    {
        sprite.material.SetFloat("_LineWidth", _value);
    }

}
