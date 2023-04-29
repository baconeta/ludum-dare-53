using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    private BoatMovement _boatMovement;

    // Start is called before the first frame update
    void Awake()
    {
        _boatMovement = GetComponent<BoatMovement>();
    }

    void StartVoyage()
    {
        GameState.GameStates currentState = GameState._instance.CurrentState;
        
        
    }
}
