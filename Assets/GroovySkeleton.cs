using Managers;
using Spawnables;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroovySkeleton : MonoBehaviour
{
    private Transform _target;
    private bool hasThrown;
    public float throwDistance;
    public GameObject throwPrefab;
    public Transform throwStartPoint;
    public bool triggerOnce;
    [Tooltip("Only if TriggerOnce=False")]
    public float triggerCooldown;
    public float cooldown;

    // Start is called before the first frame update
    void Start()
    {
        _target = GameObject.FindWithTag("Ferry").transform;
        cooldown = triggerCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameStateManager.Instance.IsGameActive())
        {
            if(Vector3.Distance(transform.position, _target.position) < throwDistance)
            {
                if (triggerOnce)
                {
                    if (!hasThrown)
                        ///TODO Replace with animation event
                        Throw();
                }
                else if (cooldown <= 0)
                {
                  
                    Throw();
                }
            }

            if(cooldown > 0)
            {
                cooldown -= Time.deltaTime;
                if (cooldown < 0) cooldown = 0;
            }
                
            
            
        }
    }

    void Throw()
    {
        if(triggerOnce)
            hasThrown = true;
        else 
            cooldown = triggerCooldown;
        //Need to instantiate on animation event.
        Yeet newYeet = Instantiate(throwPrefab, transform).GetComponent<Yeet>();
        newYeet.YeetethMySkull();
        newYeet.transform.position = throwStartPoint.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, throwDistance);
        }
}
