using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour {
    public int playerNumber;

    //public List<int> enemy = new List<int>();
    /// <summary>
    /// Method is called every turn. Allows player to interact with his units.
    /// </summary>
    public abstract void Play();
}
