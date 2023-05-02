using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTrigger : MonoBehaviour
{
    Animator _animator;
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    public void OnResetTrigger(string val)
    {
        _animator.ResetTrigger(val);
    }
    
}
