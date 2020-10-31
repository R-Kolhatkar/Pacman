using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node : MonoBehaviour
{
    public node[] neighbours;
    public Vector2[] validDirections;

    // Start is called before the first frame update
    void Start()
    {
        // Set all possible directions from current node
        // based on difference in position from each neighbour
        validDirections = new Vector2[neighbours.Length];

        for(int i = 0; i < neighbours.Length; i++)
        {
            node neighbour = neighbours[i];
            Vector2 temp = neighbour.transform.localPosition - transform.localPosition;

            validDirections[i] = temp.normalized;
        }
    }
}
