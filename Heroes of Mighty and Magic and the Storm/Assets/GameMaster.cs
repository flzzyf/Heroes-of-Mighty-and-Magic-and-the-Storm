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
	}

    public void UnitInteract(GameObject _origin, GameObject _target)
    {
        _origin.GetComponent<Unit>().FaceTarget(_target);
        _target.GetComponent<Unit>().FaceTarget(_origin);
    }

}
