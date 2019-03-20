using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

    public GameObject[] tilesPrefabs, powerUpsPrefabs;
    public int amountOfTilesOnScreen;
    public List<GameObject> tiles, powerUps;

    private Transform playerTransform;
    private float spawnZ = -5f;
    private float tileLenghtZ = 7.5f;
    private float safeZone = 15.0f;
    private int lastPrefabIndex = 0;




    // Use this for initialization
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        SpawnTile(0);
        SpawnTile(0);
        for (int i = 2; i < amountOfTilesOnScreen; i++)
        {
            SpawnTile(RandomPrefabIndex());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (playerTransform.position.z - safeZone > (spawnZ - amountOfTilesOnScreen * tileLenghtZ))
        {
            SpawnTile(RandomPrefabIndex());
            DeleteTile();

        }
        if(powerUps.Count>=1&& playerTransform.position.z -safeZone >powerUps[0].transform.position.z)
        {
            DeletePowerUp();
        }
    }



    void SpawnTile(int prefabIndex = -1)
    {
        GameObject go;
        go = Instantiate(tilesPrefabs[prefabIndex]) as GameObject;
        go.transform.SetParent(transform);
        go.transform.position = Vector3.forward * spawnZ;
        tiles.Add(go);
        //Decide if to spawn a PowerUp
        if (prefabIndex>=1)
        {
            if (UnityEngine.Random.Range(0, 100) <= 10)
            {
                
                
                int powerUpIndex = UnityEngine.Random.Range(0, powerUpsPrefabs.Length);
                Debug.Log("Spawn powerUp with index" + powerUpIndex);
                
                SpawnPowerUp(powerUpIndex, spawnZ);
                
            }
            else
            {
                Debug.Log("Nah... No powerUps this time bro.");
            }
        }
        spawnZ += tileLenghtZ;
    }

    void DeleteTile()
    {
         Destroy(tiles[0]);
         tiles.RemoveAt(0);
    }

    private int RandomPrefabIndex()
    {
         if (tilesPrefabs.Length <= 1)
             return 0;
        int randomIndex = lastPrefabIndex;
        while (randomIndex == lastPrefabIndex)
        {
            randomIndex = UnityEngine.Random.Range(0,tilesPrefabs.Length);
        }
        lastPrefabIndex = randomIndex;
        return randomIndex;
    }

    void SpawnPowerUp(int powerUpIndex, float positionZ)
    {
        if (powerUpsPrefabs.Length <= 1)
            return ;
        int positionX;
        positionX = UnityEngine.Random.Range(-1, 1);
        GameObject go;       
        go = Instantiate(powerUpsPrefabs[powerUpIndex]) as GameObject;
        go.transform.SetParent(transform);
        go.transform.position = new Vector3(positionX, 0, positionZ);
        powerUps.Add(go);
        
    }
   
    void DeletePowerUp()
    {
        Destroy(powerUps[0]);
        powerUps.RemoveAt(0);
    }
   
}
