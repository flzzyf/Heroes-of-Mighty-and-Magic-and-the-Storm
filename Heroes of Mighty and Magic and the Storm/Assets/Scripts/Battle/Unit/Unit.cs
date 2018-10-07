using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Anim { idle, walk, attack, flip, death, defend, hit }

public class Unit : NodeObject
{
    public UnitType type;

    public SpriteRenderer sprite;
    public Animator animator;
    public Text text_number;
    public GameObject ui;

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

    [HideInInspector]
    public int originalNum;

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

    bool facingRight { get { return sprite.flipX == false; } }

    IEnumerator FlipWithAnimation()
    {
        PlayAnimation(Anim.flip);

        yield return new WaitForSeconds(UnitAttackMgr.instance.animTurnbackTime / 2);

        sprite.flipX = !sprite.flipX;
    }

    void Flip()
    {
        sprite.flipX = !sprite.flipX;
    }

    public void SetFacing(int _facing)
    {
        bool flipX = _facing == 0 ? false : true;
        sprite.flipX = flipX;
    }

    public bool RestoreFacing()
    {
        bool playerFacingRight = player == 0 ? true : false;

        if (playerFacingRight != facingRight)
        {
            StartCoroutine(FlipWithAnimation());
            return true;
        }

        return false;
    }

    //面向目标（是否有转身动画）
    public bool FaceTarget(Vector2 _target, bool _flip = false)
    {
        bool targetInTheRight = (_target.x > transform.position.x) ? true : false;

        //朝向和目标相反，需要转身
        if (targetInTheRight != facingRight)
        {
            if (_flip)
            {
                StartCoroutine(FlipWithAnimation());
                return true;
            }
            else
            {
                Flip();
            }
        }
        return false;
    }

    public bool FaceTarget(Unit _target, bool _flip = false)
    {
        return FaceTarget(_target.transform.position, _flip);
    }
    #endregion

    public float GetAnimationLength(string _anim)
    {
        int index = animIndex[_anim];

        return animator.runtimeAnimatorController.animationClips[index].length;
    }

    public void PlayAnimation(Anim _anim, bool _play = true)
    {
        if (_anim == Anim.walk)
        {
            animator.SetBool("walking", _play);
        }
        else if (_anim == Anim.attack)
        {
            animator.Play("Attack");
        }
        else if (_anim == Anim.flip)
        {
            animator.Play("Flip");
        }
        else if (_anim == Anim.death)
        {
            animator.Play("Death");
        }
        else if (_anim == Anim.defend)
        {
            animator.Play("Defend");
        }
        else if (_anim == Anim.hit)
        {
            animator.Play("Hit");
        }
    }
    #region Number and HP
    //初始总共血量
    public int originalHp { get { return originalNum * type.hp; } }
    //当前总共血量
    public int totalHp { get { return ((num - 1) * type.hp) + currentHP; } }
    //失去的血量
    public int missedHp { get { return originalHp - totalHp; } }

    public void SetNum(int _amount)
    {
        num = _amount;
        text_number.text = _amount.ToString();
    }

    public void ChangeNum(int _amount)
    {
        int n = num + _amount;

        SetNum(n);
    }

    public void SetHp(int _amount)
    {
        currentHP = _amount;
    }

    void ChangeHp(int _amount)
    {
        SetHp(currentHP + _amount);
    }

    public int ModifyHp(int _amount, bool _canResurrect = false)
    {
        if (_amount > 0)
        {
            //大于0为治疗
            return RestoreHp(_amount, _canResurrect);
        }
        else if (_amount < 0)
        {
            //小于0为伤害
            return TakeDamage(_amount * -1);
        }

        return 0;
    }
    //恢复生命，返回复活数量
    int RestoreHp(int _amount, bool _canResurrect = false)
    {
        //回复之后的生命值
        int hp = currentHP + _amount;

        //回复不溢出或者无法复活，直接设血为不超过最大生命的量
        if (hp <= type.hp || !_canResurrect)
        {
            SetHp(Mathf.Min(type.hp, hp));
            return 0;
        }
        else
        {
            //可以复活，而且血量超过最大生命
            SetHp(hp % type.hp);
            //int resurrectNum = (hp - hp % type.hp) / type.hp + 1;
            //int resurrectNum = (hp - type.hp) / type.hp + 1;
            int resurrectNum = hp % type.hp > 0 ? hp / type.hp : hp / type.hp - 1;
            ChangeNum(resurrectNum);
            return resurrectNum;
        }
    }
    //造成伤害，返回单位死亡数量
    int TakeDamage(int _amount)
    {
        int remainHpTotal = totalHp - _amount;
        //如果致命
        if (remainHpTotal <= 0)
        {
            int deathNum = num;
            Death();

            return deathNum;
        }
        else
        {
            //不致命
            int deathNum = _amount / type.hp;
            int remainHp = remainHpTotal % type.hp;
            if (remainHp == 0) remainHp = type.hp;
            print(remainHp);

            if (deathNum != 0)
                ChangeNum(deathNum * -1);
            SetHp(remainHp);
            return deathNum;
        }
    }
    #endregion
    void Death()
    {
        SetNum(0);
        SetHp(0);

        BattleManager.Instance().unitActionList.Remove(this);
        BattleManager.Instance().unitActionOrder.Remove(this);
        BattleManager.Instance().waitingUnitList.Remove(this);

        dead = true;

        BattleManager.instance.units[player].Remove(this);

        BattleManager.Instance().UnlinkNodeWithUnit(this);

        animator.Play("Death");
        ui.SetActive(false);

        sprite.sortingLayerName = "DeadUnit";
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
        sprite.material.SetFloat("_LineWidth", 13);

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
