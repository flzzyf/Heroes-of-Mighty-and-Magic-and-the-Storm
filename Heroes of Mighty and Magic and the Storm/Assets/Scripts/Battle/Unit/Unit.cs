using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Anim { idle, walk, attack }

public class Unit : NodeObject
{
    public UnitType type;

    public SpriteRenderer sprite;
    public Animator animator;
    public Text text_number;
    public GameObject ui;

    //朝向，-1为左，1为右
    int facing = 1;

    //单位属性
    [HideInInspector]
    public int speed, att, def, currentHP, num;
    [HideInInspector]
    public Vector2 damage;
    [HideInInspector]
    public int retaliations;

    Dictionary<string, int> animIndex = new Dictionary<string, int>{
        {"move", 2}, {"attack", 1}
    };

    bool outlineFlashing;
    bool fading;

    [HideInInspector]
    public bool dead;

    public void Init()
    {
        if (type != null)
            InitUnitType();

    }

    private void Update()
    {
        if (outlineFlashing)
        {
            Color color = sprite.material.GetColor("_Color");

            float alpha = color.a;

            if (alpha > GameSettings.instance.outlineFlashRangeMax || alpha < GameSettings.instance.outlineFlashRangeMin)
            {
                alpha = Mathf.Clamp(alpha, GameSettings.instance.outlineFlashRangeMin, GameSettings.instance.outlineFlashRangeMax);

                fading = !fading;
            }

            int sign = fading ? -1 : 1;
            ChangeOutline(alpha + sign * GameSettings.instance.outlineFlashSpeed * Time.deltaTime);
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
    #region Facing
    public void Flip()
    {
        sprite.flipX = !sprite.flipX;
        facing *= -1;
    }

    public void RestoreFacing()
    {
        int playerFacing = player == 0 ? 1 : -1;

        if (playerFacing != facing)
            Flip();
    }

    public bool FaceTarget(Vector2 _target)
    {
        int targetInTheRight = (_target.x > transform.position.x) ? 1 : -1;

        if (targetInTheRight != facing)
        {
            Flip();
            return true;
        }
        return false;
    }

    public bool FaceTarget(GameObject _target)
    {
        return FaceTarget(_target.transform.position);
    }
    #endregion

    public float GetAnimationLength(string _anim)
    {
        //print(_anim);
        int index = animIndex[_anim];

        return animator.runtimeAnimatorController.animationClips[index].length;
    }

    public void PlayAnimation(Anim _anim, bool _play = true)
    {
        if (_anim == Anim.idle)
        {

        }
        else if (_anim == Anim.walk)
        {
            animator.SetBool("walking", _play);
        }
        else if (_anim == Anim.attack)
        {
            animator.Play("Attack");
        }
    }
    #region Number and HP
    public void ChangeNum(int _amount)
    {
        num = _amount;
        text_number.text = _amount.ToString();
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
    //造成伤害，导致单位死亡
    public bool TakeDamage(int _amount)
    {
        if (_amount > (num - 1) * type.hp + currentHP)
        {
            //死了
            Death();

            return true;
        }

        if (_amount < currentHP)
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
        return false;

        //print("剩余生命:" + currentHP);
    }
    #endregion
    void Death()
    {
        BattleManager.Instance().unitActionList.Remove(this);
        BattleManager.Instance().unitActionOrder.Remove(this);

        dead = true;

        BattleManager.instance.units[player].Remove(this);

        BattleManager.Instance().UnlinkNodeWithUnit(this);

        animator.Play("Death");
        ui.SetActive(false);
    }

    #region Outline
    void ChangeOutline(float _value = 0)
    {
        Color color = sprite.material.GetColor("_Color");
        color.a = _value;
        sprite.material.SetColor("_Color", color);
    }

    public void OutlineFlashStart()
    {
        sprite.material.SetFloat("_LineWidth", 5);

        outlineFlashing = true;
        fading = true;
        ChangeOutline(GameSettings.instance.outlineFlashRangeMax);
    }

    public void OutlineFlashStop()
    {
        sprite.material.SetFloat("_LineWidth", 0);

        outlineFlashing = false;
        ChangeOutline(0);
    }

    public void ChangeOutlineColor(string _color)
    {
        for (int i = 0; i < BattleManager.instance.outlineColor.Length; i++)
        {
            if (_color == BattleManager.instance.outlineColor[i].name)
            {
                sprite.material.SetColor("_Color", BattleManager.instance.outlineColor[i].color);
                return;
            }
        }

        print("未能找到颜色");
    }

    #endregion
    //拥有特质
    public bool PossessTrait(string _name)
    {
        return type.traits.Contains(TraitManager.instance.GetTrait(_name));
    }
}
