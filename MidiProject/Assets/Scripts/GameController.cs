using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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
    // Moveing Note Signals
    public GameObject cubeNote;
    private GameObject[] cubeNotes = new GameObject[4];
    [SerializeField]
    private double gain = 0;

    // Not a "true" state machine but it'll do for tracking what
    // scene we should be in and what to do ext...
    private string STATE = "MAPPED_SONG";

    // Mapped Song to be played
    private MappedSong song = null;
    private string[] maps = new string[] { "AtDoomsGate", "CanonInD", "Test" };

    private NeckHolder neckHolder;
    private GameObject[] strings = new GameObject[6];
    private GameObject[] notes = new GameObject[36];

    // Song Game Loop Data
    private int tempo;
    public float timeRate = 3f;
    private float clock;
    private float noteTimer;

    // Mapped Song Vars
    private bool hasHitnote;

    private void Awake()
    {
        clock = timeRate;
        noteTimer = timeRate;
        // Innits all strings and notes for standard tunnings
        neckHolder = new NeckHolder(Tunnings.standardTunning);
        // Spawn in the string and note prefabs
        SpawnStrings(stringSpawner.transform.position);
        SpawnNotes(noteSpawner.transform.position);        
    }

    [Obsolete]
    private void Update()
    {
        try
        {
            switch (STATE)
            {
                case "MENU":
                    Menu();
                    break;

                case "FREE_PLAY":
                    FreePlay();
                    break;

                case "MAPPED_SONG":
                    // Instantiates "song" as a MappedSong obj
                    LoadMap("Test");
                    if (noteTimer <= clock)
                    {
                        DestroyNoteCubes();
                        hasHitnote = false;
                        song.PlayNote();
                        clock -= timeRate;
                        int cubeIndex = (int)UnityEngine.Random.RandomRange(0f, 36f);
                        SpawnCubeNotes(10, cubeIndex);
                        MoveCubeNotesToNote(cubeIndex);
                    }
                    HasPlayerHitNote();
                    clock += Time.deltaTime;
                    break;

                case "SCORE":
                    Score();
                    break;

                default:
                    // Goes back to menu 
                    Menu();
                    break;
            }
        } catch (Exception e)
        {   // Currently throws when the stack runs dry...which is good...kinda
            Debug.Log("State switch-case broke :(");
            Debug.LogException(e, this);
            Menu();
        }
    }

    // TODO \\
    // This will get move complex as I worry about
    // note timmings
    private void SpawnCubeNotes(int spawnDistance, int noteIndex)
    {
        Vector3 spawn = notes[noteIndex].transform.position;
        Quaternion q = new Quaternion(0, 0, 0, 1);

        // Spawn from front, going round clockwise
        spawn.z += spawnDistance;
        cubeNotes[0] = Instantiate(cubeNote, spawn, q);

        spawn = notes[noteIndex].transform.position;
        spawn.x += spawnDistance;
        cubeNotes[1] = Instantiate(cubeNote, spawn, q);

        spawn = notes[noteIndex].transform.position;
        spawn.z -= spawnDistance;
        cubeNotes[2] = Instantiate(cubeNote, spawn, q);

        spawn = notes[noteIndex].transform.position;
        spawn.x -= spawnDistance;
        cubeNotes[3] = Instantiate(cubeNote, spawn, q);
    }
    private void MoveCubeNotesToNote(int noteIndex)
    {
        for(int i = 0; i < cubeNotes.Length; i++)
        {
            Debug.Log("started");
            StartCoroutine(MoveCube(cubeNotes[i], notes[noteIndex].transform.position, 10));
        }
    }
    private void DestroyNoteCubes()
    {
        StopAllCoroutines();
        if(cubeNotes.Length > 0)
        {
            for(int i = 0; i < cubeNotes.Length; i++)
            {
                Destroy(cubeNotes[i]);
            }

        }
    }

    private int GetPlayedNoteIndex()
    {
        int pin = GetPlayedString();
        int index = neckHolder.GetStrings()[pin].GetNoteIndex(inputVoltage);
        // Needs to loop through the array of 36 in 6's to find the note on which string
        if(pin == 0)
        {
            pin = 1;
        }
        index = (pin * 6) + index;

        return index;
    }
    private int GetTargetNeckNoteIndex()
    {
        string targetNoteName = song.currentNote.noteName.ToUpper() + song.currentNote.noteOctave;
        for(int i = 0; i < notes.Length; i++)
        {
            if (notes[i].name == targetNoteName)
            {
                return i;                
            }
        }
        return -1;
    }
    private void HasPlayerHitNote()
    {
        int targetNote = GetTargetNeckNoteIndex();
        int playedNote = GetTargetNeckNoteIndex();
    }
 
    // TODO \\
    public void Menu()
    {
        // Open's game menu and changes state to either
        // Free play, Mapped Song or Score
    }
    // TODO \\
    public void Score()
    {
        // Will display the score board
    }

    private void LoadMap(string _name)
    {
        if (song == null)
        {
            song = new MappedSong(_name);
        }
    }

    public void FreePlay()
    {
        int pin = GetPlayedString();
        Note playedNote = neckHolder.GetStrings()[pin].GetNote(inputVoltage);
        PlayNote(playedNote);
    }
    private IEnumerator MoveCube(GameObject cube, Vector3 destination, float speed)
    {

        while (cube.transform.position != destination)
        {
            cube.transform.position = Vector3.MoveTowards(cube.transform.position, destination, speed * Time.deltaTime);
            yield return null;
        }
    }

    /// <summary>
    /// Played the given note and turns the others off
    /// </summary>
    /// <param name="n">Note object</param>
    private void PlayNote(Note n)
    {
        foreach(GameObject note in notes)
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

// TESTING

//// TESTING MOVEMEANT
//public GameObject testCubeOne;
//public GameObject testCubeTwo;
//public GameObject testCubeTargetLocation;
//IEnumerator currentMoveRoutine;
//if (Input.GetKeyDown(KeyCode.E))
//{
//    if(currentMoveRoutine != null)
//    {
//        StopCoroutine(currentMoveRoutine);
//    }
//    currentMoveRoutine = MoveCube(testCubeOne, testCubeTargetLocation.transform.position);
//    StartCoroutine(currentMoveRoutine);
//}
//if (Input.GetKeyDown(KeyCode.Q))
//{
//    StopCoroutine(currentMoveRoutine);
//}