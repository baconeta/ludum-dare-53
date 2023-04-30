using Audio;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour
{
    void Start()
    {
        AudioWrapper.Instance.PlaySound("game-background-music");
    }
}
