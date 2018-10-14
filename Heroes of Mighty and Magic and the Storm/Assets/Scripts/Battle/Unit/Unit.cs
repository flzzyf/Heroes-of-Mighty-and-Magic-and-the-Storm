using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Unit : NodeObject
{
    public UnitType type;

    public SpriteRenderer sprite;
    public Animator animator;
    public Text text_number;
    public GameObject UI;

    //单位属性
    [HideInInspector]
    public int speed, att, def, currentHP, num, ammo;
    [HideInInspector]
    public Vector2Int damage;
    [HideInInspector]
    public int retaliations;

    [HideInInspector]
    public bool dead;

    [HideInInspector]
    public int originalNum;

    public void Init()
    {
        if (type != null)
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
        ammo = type.ammo;
    }
    #region Facing

    public bool facingRight { get { return sprite.flipX == false; } }

    IEnumerator FlipWithAnimation()
    {
        UnitAnimMgr.instance.PlayAnimation(this, Anim.Flip);

        yield return new WaitForSeconds(UnitAttackMgr.instance.animTurnbackTime / 2);

        sprite.flipX = !sprite.flipX;
    }

    void Flip()
    {
        sprite.flipX = !sprite.flipX;
        Vector3 UIPos = UI.transform.GetComponent<RectTransform>().localPosition;
        UIPos.x *= -1;
        UI.transform.GetComponent<RectTransform>().localPosition = UIPos;
    }

    public void SetFacing(int _facing)
    {
        bool flipX = _facing == 0 ? false : true;
        if (sprite.flipX != flipX)
            Flip();
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
        int side = player == BattleManager.players[0] ? 0 : 1;
        BattleResultMgr.instance.ChangeCasualties(side, type, _amount);

        SetNum(num + _amount);
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
            int deathNum = 0;
            if (_amount > currentHP)
            {
                deathNum = 1 + (_amount - currentHP) / type.hp;
            }
            int remainHp = remainHpTotal % type.hp;
            if (remainHp == 0) remainHp = type.hp;

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

        //播放死亡音效
        GameManager.instance.PlaySound(type.sound_death);
        animator.Play("Death");
        UI.SetActive(false);

        GetComponent<AxisZControl>().offsetZ = 1;
    }

    //拥有特质
    public bool PossessTrait(string _name)
    {
        return type.traits.Contains(TraitManager.instance.GetTrait(_name));
    }

    //是地面行走者（非飞行和瞬移）
    public bool isWalker { get { return !PossessTrait("Flying") && !PossessTrait("Teleporting"); } }

    [HideInInspector]
    public List<Behavior> behaviors = new List<Behavior>();
    public void AddBehavior(Behavior _behavior)
    {
        behaviors.Add(_behavior);

        _behavior.Init(this);
        _behavior.Add();
    }

    public void RemoveBehavior(Behavior _behavior)
    {
        for (int i = 0; i < behaviors.Count; i++)
        {
            if (behaviors[i] == _behavior)
            {
                behaviors[i].Remove();

                behaviors.Remove(behaviors[i]);
            }
        }
    }

    public bool PossessBehavior(Behavior _behavior)
    {
        for (int i = 0; i < behaviors.Count; i++)
        {
            if (behaviors[i] == _behavior)
            {
                behaviors[i].duration--;
                print(behaviors[i].duration);
                return true;
            }
        }

        return false;
    }
}
