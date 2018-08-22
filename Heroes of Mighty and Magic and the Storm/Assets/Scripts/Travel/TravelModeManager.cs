using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelModeManager : Singleton<TravelModeManager>
{


	void Start () 
	{
        NodeManager.Instance().GenerateNodes();
        MapManager.Instance().GenerateMap();
	}
	
	void Update () 
	{
		
	}
}
