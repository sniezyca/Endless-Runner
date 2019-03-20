using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour {

    public GameObject[] PowerUps;
    
    public List<GameObject> powers;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Disappear(GameObject power)
    {
        power.SetActive(false);
    }
}
