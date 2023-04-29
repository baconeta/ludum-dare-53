using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatCapacity : MonoBehaviour
{
    [Tooltip("The amount of souls that can fit at the boat when the game starts.")]
    private int startingCapacity = 6;

    [Tooltip("Does the boat start loaded to capacity? or does it start empty?")]
    private Boolean doesStartLoaded = false;


    public int actualCapacity { get; private set; }

    public int currentLoad { get; private set; }


    public static event Action OnBoatDestroyed;


    // Start is called before the first frame update
    void Start()
    {
        ResetCapacity();
        currentLoad = doesStartLoaded ? startingCapacity : 0;
    }

    /// <summary>
    /// Reduce the capacity of the boat.
    /// </summary>
    /// <param name="amount">By how many units should the maximum capacity be reduced.</param>
    /// <returns>The number of units that were lost.</returns>
    public int ReduceCapacity(int amount = 1)
    {
        actualCapacity = actualCapacity - amount;
        if (actualCapacity == 0)
        {
            OnBoatDestroyed?.Invoke();
        }
        else if (actualCapacity < 0)
        {
            actualCapacity = 0;
        }
        return TrimLoad();
    }

    /// <summary>
    /// Increase the capacity of the boat.
    /// </summary>
    /// <param name="amount">By how many units should the maximum capacity be increased.</param>
    public void IncreaseCapacity(int amount = 1)
    {
        actualCapacity = actualCapacity + amount;
    }

    /// <summary>
    /// Reset the actualCapacity to the startingCapacity. Trim excess currentLoad to match new capacity.
    /// <returns>The number of units that were lost.</returns>
    /// </summary>
    public int ResetCapacity()
    {
        actualCapacity = startingCapacity;
        return TrimLoad();
    }

    /// <summary>
    /// Add a number of units to the load of the boat. Excess units that couldn't be loaded are returned.
    /// </summary>
    /// <param name="loadToAdd">How many units are we trying to add to the load.</param>
    /// <returns>The number of units that were not able to fit onto the boat due to load capacity limits.</returns>
    public int AddLoad(int loadToAdd)
    {
        // If we're trying to overload the boat.
        if (loadToAdd > (actualCapacity - currentLoad))
        {
            int excess = loadToAdd + currentLoad - actualCapacity;
            currentLoad = actualCapacity;
            return excess;
        } else
        {
            currentLoad = currentLoad + loadToAdd;
            return 0;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>The number of units that were lost.</returns>
    private int TrimLoad()
    {
        // Excess load is lost.
        if (currentLoad > actualCapacity)
        {
            int excess = currentLoad - actualCapacity;
            currentLoad = actualCapacity;
            return excess;
        } else { return 0; }
        
    }
}
