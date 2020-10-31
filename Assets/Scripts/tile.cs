using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tile : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isPortal;

    public bool isPellet;
    public bool isPelletEnergizer;
    public bool didConsume;

    public bool isGhostHouseEntrance;
    public bool isGhostHouse;

    public bool isResource;
    public int resourceValue;

    public GameObject portalReceiver;
}
