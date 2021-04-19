using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//använt för att testa sak bara, varsågod att radera eller ändra för att testa saker själv.
public class testScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TriggerEnter");
    }

}
