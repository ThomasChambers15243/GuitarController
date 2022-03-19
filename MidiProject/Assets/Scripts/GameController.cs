using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject stringSpawner;
    public GameObject notePrefab;
    public GameObject stringPrefab;
    public GameObject noteSpawner;
    // WILL BE VOLTAGE FROM GUITAR BUT FOR NOW ITS FROM ME
    public float inputVoltage;
    // Gaps between strings and notes
    public float stringGapScalar;
    public float noteGapScalar;

    
    private NeckHolder neckHolder;
    private GameObject[] strings = new GameObject[6];
    private GameObject[] notes = new GameObject[36];

    // Start is called before the first frame update
    void Start()
    {
        // Innits all strings and notes for standard tunnings
        neckHolder = new NeckHolder(Tunnings.standardTunning);        
        SpawnStrings(stringSpawner.transform.position);
        SpawnNotes(noteSpawner.transform.position);
        // This will be relitive to the pin
        int index = 0;
        if (neckHolder.GetStrings()[0].GetNote(inputVoltage) != null)
        {
            Note playedNote = neckHolder.GetStrings()[0].GetNote(inputVoltage);
        }


        

    }

    private void PlayNote(Note n)
    {

    }


    /// <summary>
    /// Spawns strings on the backing
    /// </summary>
    void SpawnStrings(Vector3 spawnPosition)
    {
        for(int i = 0; i < 6; i++)
        {      
            Quaternion q = new Quaternion(0.707106829f, 0, 0, 0.707106829f);
            Vector3 spawn = new Vector3(spawnPosition.x + i * stringGapScalar, spawnPosition.y, spawnPosition.z);
            strings[i] = Instantiate(stringPrefab, spawn , q);
            strings[i].GetComponent<Transform>().SetParent(stringSpawner.transform);
        }
    }

    /// <summary>
    /// Spawns note on the strings on the backing
    /// </summary>
    void SpawnNotes(Vector3 spawnPosition)
    {        
        int noteCounter = 0;
        for(int i = 0; i < 6; i++)
        {            
            for (int j = 0; j < 6; j++)
            {
                Quaternion q = new Quaternion(0, 0, 0, 1);
                Vector3 spawn = new Vector3(spawnPosition.x + i * stringGapScalar, spawnPosition.y, spawnPosition.z + j * noteGapScalar);
                notes[noteCounter] = Instantiate(notePrefab, spawn, q);
                notes[noteCounter].GetComponent<Transform>().SetParent(noteSpawner.transform);
                noteCounter += 1;
            }
        }
    }

}
