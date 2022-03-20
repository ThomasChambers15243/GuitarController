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

    [SerializeField]
    private double gain = 1;

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
    }

    private void Update()
    {
        int pin = GetPlayedString();
        float testVal = 4.64f;
        Note playedNote = neckHolder.GetStrings()[pin].GetNote(inputVoltage);
        PlayNote(playedNote);
    }

    private void PlayNote(Note n)
    {
        foreach(GameObject note in notes)
        {
            try
            {
                if(n.GetNameWithOctave() == note.name)
                {
                    note.GetComponent<GenerateNote>().playEffect = true;
                }
                else
                {
                    note.GetComponent<GenerateNote>().playEffect = false;
                }
            }
            catch (System.NullReferenceException err)
            {
                note.GetComponent<GenerateNote>().playEffect = false;                
            }
        }
    }

    // Returns the index of the string that was played relitive to the input pin
    // Pin A0 == string 0
    private int GetPlayedString()
    {
        return 0;
    }

    private void ChangeMaterialTo(GameObject g, string path)
    {
        g.GetComponent<Renderer>().material = Resources.Load(path, typeof(Material)) as Material;
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
                Note currentNote = neckHolder.GetStrings()[i].notes[j];
                Quaternion q = new Quaternion(0, 0, 0, 1);
                Vector3 spawn = new Vector3(spawnPosition.x + i * stringGapScalar, spawnPosition.y, spawnPosition.z + j * noteGapScalar);
                notes[noteCounter] = Instantiate(notePrefab, spawn, q);
                notes[noteCounter].name = currentNote.GetNameWithOctave();
                notes[noteCounter].GetComponent<Transform>().SetParent(noteSpawner.transform);
                notes[noteCounter].AddComponent<GenerateNote>();
                notes[noteCounter].GetComponent<GenerateNote>().Innit(currentNote.GetFreq(), gain);
                noteCounter += 1;
            }
        }
    }

}
