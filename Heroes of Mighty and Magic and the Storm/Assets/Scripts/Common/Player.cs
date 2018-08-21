using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public int id = 0;

    #region Singleton
    [HideInInspector]
    public static Player instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }
    #endregion

	void Start () {
		
	}

}
