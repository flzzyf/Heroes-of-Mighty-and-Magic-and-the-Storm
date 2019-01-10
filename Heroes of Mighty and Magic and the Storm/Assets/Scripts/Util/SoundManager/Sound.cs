using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sound")]
public class Sound : ScriptableObject
{
    public AudioClip[] clips;

    [Range(0f, 1.5f)]
    public float volume = 1;
    [Range(.1f, 3f)]
    public float pitch = 1;

    public bool loop;

    public SoundGroup group;

    public float startingTime;
}
