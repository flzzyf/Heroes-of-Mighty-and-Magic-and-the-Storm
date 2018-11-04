using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : NodeObject, MovableNode
{
    [HideInInspector]
    public int player;

    public UnitType type;

    public SpriteRenderer sprite;
    public Animator animator;
    public GameObject UI;
    public Text text_number;

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

    public List<Behavior> behaviors = new List<Behavior>();

    public void Init()
    {
        if (type != null)
            InitUnitType();

        Vector3 pos = UI.GetComponent<RectTransform>().localPosition;
        pos.z = -500;
        UI.GetComponent<RectTransform>().localPosition = pos;

        UpdateUI();
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

    public Vector2 UIOffset = new Vector2(100, 39);
    public void UpdateUI()
    {
        int offsetX = facingRight ? 1 : -1;
        if (nodeItem == null)
        {
            SetUIPos(new Vector2(UIOffset.x * offsetX, UIOffset.y));
        }
        else
        {
            //如果单位面前的节点有单位，UI移动到源点
            NodeItem item = BattleManager.instance.map.GetNodeItem(nodeItem.pos + new Vector2Int(offsetX, 0));
            if (item.nodeObject != null && item.nodeObject.nodeObjectType == NodeObjectType.unit)
            {
                SetUIPos(new Vector2(0, 0));
            }
            else
            {
                SetUIPos(new Vector2(UIOffset.x * offsetX, UIOffset.y));
            }
        }

    }

    void SetUIPos(Vector2 _pos)
    {
        UI.GetComponent<RectTransform>().localPosition = _pos;
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
            int resurrectNum;
            if (hp % type.hp > 0)
            {
                resurrectNum = hp / type.hp;
            }
            else
            {
                resurrectNum = hp / type.hp - 1;
            }
            //不能超过初始生命
            int maxResurrect = originalNum - num;
            resurrectNum = Mathf.Min(maxResurrect, resurrectNum);

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

    //是地面行走者（非飞行和瞬移）
    public bool isWalker
    {
        get
        {
            return !TraitManager.PossessTrait(this, "Flying") &&
                   !TraitManager.PossessTrait(this, "Teleporting");
        }
    }

    public void TowardNextNode(NodeItem _node)
    {
        print("朝向下一节点");
    }

    public float UnitActualSpeed
    {
        get
        {
            if (!TraitManager.PossessTrait(this, "Flying"))
                return BattleManager.instance.unitSpeedOriginal +
                        BattleManager.instance.unitSpeedMultipler * speed;
            else
                return BattleManager.instance.unitSpeedOriginal +
                        BattleManager.instance.flyingSpeedmultipler * speed;
        }
    }
}
