using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.Play("game-background-music");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
