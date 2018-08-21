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

    public UnitType type;

    //----------------------
    public GameObject[] gog;

    public GameObject unitPrefab;

    #region 游戏暂停系统
    int pause = 0;

    public void Pause(){
        pause++;
    }

    public void Unpause(){
        pause--;
    }

    public bool isPause(){
        return pause > 0;
    }
    #endregion

    void Start ()
    {
        Cursor.visible = false;
	}

	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {

        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            BattleManager.instance.BattleStart();

        }
	}


    public GameObject CreateUnit(UnitType _type, Vector3 _pos, int _num = 1, int _flip = 0)
    {
        GameObject go = Instantiate(unitPrefab, _pos, Quaternion.identity);
        Unit unit = go.GetComponent<Unit>();
        unit.type = _type;
        unit.InitUnitType();
        unit.ChangeNum(_num);
        if (_flip == 1)
            unit.Flip();

        return go;
    }

}
