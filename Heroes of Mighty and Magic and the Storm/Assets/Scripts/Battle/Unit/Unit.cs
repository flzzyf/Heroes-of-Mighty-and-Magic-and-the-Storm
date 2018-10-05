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
    }
    #region Number and HP
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

    public void ModifyHp(int _amount, bool _canResurrect = false)
    {
        if (_amount > 0)
        {
            //大于0为治疗
            RestoreHp(_amount, _canResurrect);
        }
        else if (_amount < 0)
        {
            //小于0为伤害
            TakeDamage(_amount * -1);
        }
    }
    //恢复生命
    void RestoreHp(int _amount, bool _canResurrect = false)
    {
        int hp = currentHP + _amount;
        if (hp <= type.hp)
        {
            SetHp(hp);
        }
        else
        {
            //治疗量超过最大生命，如果不能复活
            if (!_canResurrect)
            {
                SetHp(type.hp);
            }
            else
            {
                int resurrectCount = hp / type.hp;
                SetHp(hp % type.hp);

                //复活不能超过战斗开始时数量
                if (num + resurrectCount < originalNum)
                {
                    ChangeNum(resurrectCount);
                }
                else
                {
                    SetNum(originalNum);
                }

                print("复活个数：" + resurrectCount);
            }
        }
    }
    //造成伤害，导致单位死亡
    void TakeDamage(int _amount)
    {
        if (_amount > (num - 1) * type.hp + currentHP)
        {
            //死了
            Death();
        }

        if (_amount < currentHP)
        {
            ChangeHp(_amount * -1);
        }
        else
        {
            int deathNum = 1 + _amount / type.hp;
            ChangeNum(deathNum * -1);
            int remainHp = type.hp - (_amount - currentHP);
            ChangeHp(remainHp);
        }

        //print("剩余生命:" + currentHP);
    }
    #endregion
    void Death()
    {
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
