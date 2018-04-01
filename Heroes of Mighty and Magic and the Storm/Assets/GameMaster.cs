using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    public GameObject[] go;

	void Start () {
		
	}
	
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            UnitInteract(go[0], go[1]);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            UnitInteractEnd();
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

}
