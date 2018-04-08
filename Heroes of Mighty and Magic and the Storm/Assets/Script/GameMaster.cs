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

    public Texture2D[] mouseCursor;
    Texture2D originMouseCursor;
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

        //CreateUnit(type, Vector3.zero);
	}
	
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            //UnitInteract(go[0], go[1]);
            //print(DamageRate(2, 1));
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            //UnitInteractEnd();

        }
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

    public void ChangeMouseCursor(int _index = 0)
    {
        Cursor.SetCursor(mouseCursor[_index], Vector2.zero, CursorMode.Auto);

    }
}
