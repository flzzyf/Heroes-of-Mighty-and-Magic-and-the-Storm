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

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "MapNode")
                {
                    Debug.Log(" you clicked on " + hit.collider.gameObject.name);

                    GameObject start = MapManager.Instance().GetNodeItem(new Vector2Int(0, 0));

                    foreach (var item in MapManager.Instance().FindPath(start, hit.collider.gameObject))
                    {
                        item.GetComponentInChildren<SpriteRenderer>().color = Color.black;

                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "MapNode")
                {
                    hit.collider.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.black;
                    Node node = NodeManager.Instance().GetNode(hit.collider.gameObject.GetComponent<NodeItem>().pos);
                    node.walkable = false;
                }
            }
        }
    }

}
