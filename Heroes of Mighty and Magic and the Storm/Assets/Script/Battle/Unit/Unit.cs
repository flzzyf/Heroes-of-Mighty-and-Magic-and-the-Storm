using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{

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
    public int fightBackCount;

    [HideInInspector]
    public GameObject nodeUnit;

    Dictionary<string, string> animName = new Dictionary<string, string>{
        {"move", "walking"}, {"attack", "Attack"}
    };

    Dictionary<string, int> animIndex = new Dictionary<string, int>{
        {"move", 2}, {"attack", 1}
    };

    [HideInInspector]
    public int player;

    bool outlineFlashing;
    bool fading;

    [HideInInspector]
    public bool dead;

    void Start()
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
        fightBackCount = type.fightBackCount;
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

    public bool FaceTarget(GameObject _target)
    {
        int targetInTheRight = (_target.transform.position.x > transform.position.x) ? 1 : -1;

        if (targetInTheRight != facing)
        {
            Flip();
            return true;
        }
        return false;
    }

    public bool FaceTarget(Unit _target)
    {
        GameObject node = _target.nodeUnit;
        int targetInTheRight = (node.transform.position.x > transform.position.x) ? 1 : -1;

        if (targetInTheRight != facing)
        {
            Flip();
            return true;
        }
        return false;
    }
    #endregion

    public float GetAnimationLength(string _anim)
    {
        int index = animIndex[_anim];

        return animator.runtimeAnimatorController.animationClips[index].length;
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
    #region Number and HP
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
        BattleManager.instance.unitActionList.Remove(gameObject);
        BattleManager.instance.unitActionOrder.Remove(gameObject);

        dead = true;
        gameObject.SetActive(false);
    }

    #region Outline
    public void ChangeOutline(float _value = 0)
    {
        Color color = sprite.material.GetColor("_Color");
        color.a = _value;
        sprite.material.SetColor("_Color", color);
    }

    public void OutlineFlashStart()
    {
        outlineFlashing = true;
        fading = true;
        ChangeOutline(GameSettings.instance.outlineFlashRangeMax);
        //sprite.material.SetFloat("_LineWidth", 5);
    }

    public void OutlineFlashStop()
    {
        outlineFlashing = false;
        ChangeOutline(0);
    }
#endregion
}
