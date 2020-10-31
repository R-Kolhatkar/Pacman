using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    float randomLifeExpectancy;
    float currentLifetime;

    // Start is called before the first frame update
    void Start()
    {
        randomLifeExpectancy = Random.Range(9, 10);

        this.name = "bonusItem";

        GameObject.Find("Game").GetComponent<gameboard>().board[14, 13] = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentLifetime < randomLifeExpectancy)
        {
            currentLifetime += Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
