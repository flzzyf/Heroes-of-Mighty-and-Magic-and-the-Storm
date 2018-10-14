using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitManager : Singleton<TraitManager>
{
    public List<Trait> traits;
    [HideInInspector]
    public Dictionary<string, Trait> traitDictionary;

    void Awake()
    {
        traitDictionary = new Dictionary<string, Trait>();
        for (int i = 0; i < traits.Count; i++)
        {
            traitDictionary.Add(traits[i].name, traits[i]);
        }
    }

    public Trait GetTrait(string _name)
    {
        if (traitDictionary.ContainsKey(_name))
        {
            return traitDictionary[_name];
        }
        print("未能找到特质");
        return null;
    }
}

[System.Serializable]
public class GenericObject<T>
{
    public string name;
    public T obj;
}