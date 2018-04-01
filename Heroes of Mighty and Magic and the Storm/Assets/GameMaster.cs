using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    #region Singleton
    [HideInInspector]
    public static GameMaster instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }
    #endregion

    [HideInInspector]
    public Map map;

    public GameObject[] go;

    public GameObject unitPrefab;

    public UnitType type;

	void Start () 
    {
        map = GetComponent<Map>();

        CreateUnit(type, Vector3.zero);
	}
	
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            //UnitInteract(go[0], go[1]);
            //print(DamageRate(2, 1));
            print(DamageRate(1, 3));
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            UnitInteractEnd();
        }
	}

    void RoundStart()
    {
        
    }

    void TurnStart()
    {
        //选出速度最大单位
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

    GameObject origin, target;
    bool targetFlip;

    void UnitInteract(GameObject _origin, GameObject _target)   //交互开始
    {
        origin = _origin;
        target = _target;

        _origin.GetComponent<Unit>().FaceTarget(_target);

        targetFlip = _target.GetComponent<Unit>().FaceTarget(_origin);
    
    }

    void UnitInteractEnd()  //交互结束
    {
        if(targetFlip)
        {
            targetFlip = false;

            target.GetComponent<Unit>().Flip();
        }
    }

    public void CreateUnit(UnitType _type, Vector3 _pos)
    {
        GameObject unit = Instantiate(unitPrefab, _pos, Quaternion.identity);
        unit.GetComponent<Unit>().type = _type;
        unit.GetComponent<Unit>().InitUnitType();
    }

}
