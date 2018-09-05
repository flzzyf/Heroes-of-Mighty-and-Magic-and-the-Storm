using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public Player[] players;
}

[System.Serializable]
public class Player
{
    public int id;
    public bool isAI;

    public Vector2Int startingPoint;

    public List<GameObject> heroes;

    public int gold;
}
