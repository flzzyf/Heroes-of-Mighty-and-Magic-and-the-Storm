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
            //BattleManager.instance.units[0][0].GetComponent<Unit>().OutlineFlashStart();

            for (int i = 0; i < type.animControl.animationClips.Length; i++)
            {
                //print(type.animControl.animationClips[i].name + ":" + type.animControl.animationClips[i].length);

            }

            StartCoroutine(QWE());
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            

        }
	}

    IEnumerator QWE()
    {
        float secondPerFrame = 1 / type.animControl.animationClips[0].frameRate;
        //print(secondPerFrame);
        //print(type.animControl.animationClips[1].length);

        BattleManager.instance.units[0][0].GetComponent<Unit>().PlayAnimation("attack");

        yield return new WaitForSeconds(type.animControl.animationClips[1].length);

        print("Finish");

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
