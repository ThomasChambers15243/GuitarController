using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
    private string[] maps = new string[] { "AtDoomsGate", "CanonInD", "Test", "Test1" };

    private NeckHolder neckHolder;
    private GameObject[] strings = new GameObject[36];
    private GameObject[] notes = new GameObject[36];

    // Song Game Loop Data
    //public float timeRate = 3f;
    //private float noteTimer;
    public int cubeSpawnDistance = 10;
    public int tempo = 60;
    private bool startMap = false;
    private float clock;
    private float beat;
    private bool isPlayingMap = false;
    private bool hitNote = false;
    private float quarterNoteLength;
    private int noteCount;
    // Score vars
    private int playerScore = 0;
    private bool playerCanHitNote = false;
    private int playerAccuracy = 0;
    private IEnumerator[] cubeNotesCoroutines = new IEnumerator[4];

    // Mapped Song Vars
    private bool hasHitnote;

    private void Awake()
    {
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
                    Debug.Log("MENU: Score is " + playerScore);
                    break;

                case "FREE_PLAY":
                    FreePlay();
                    break;



                    // TODO create timmings for holding notes,
                    // have the note start when the blocks collide
                    // Maybe have a quarter note as the base timer, run the
                    // blocks of that and then reset based of the
                    // base timer + note duration
                case "MAPPED_SONG":
                    // Instantiates mapped song and game mode                    
                    if (isPlayingMap == false)
                    {
                        startMap = true;//= false;
                        LoadMap("Test");
                        quarterNoteLength = 60f / tempo;
                        beat = quarterNoteLength;
                        clock = beat;
                        isPlayingMap = true;
                        noteCount = 0;
                        playerAccuracy = 0;                        
                    }

                    // Countdown for game to start
                    //IEnumerator countdown = Countdown(3);
                    //StartCoroutine(countdown);
                    // Starts game map
                    if (startMap)
                    {
                        //StopCoroutine(countdown);
                        // Plays next note
                        if (clock >= beat)
                        {
                            // Increameants score only if the player hit the note                        
                            if (hitNote)
                            {
                                song.score += playerAccuracy;
                            }
                            // Reset all values for individual note
                            playerAccuracy = 0;
                            hitNote = false;
                            DestroyNoteCubes();
                            StopCubeNotesCoroutines();
                            // If the song is comleted, returns to the menu state
                            noteCount += 1;
                            if (noteCount == song.GetLengthOfMap())
                            {
                                // Change to game over screen
                                STATE = "MENU";
                                isPlayingMap = false;
                                playerScore = song.score;
                                break;
                            }
                            // Get the next note, reset the clock and the reset the noteCubes
                            song.PlayNote();
                            clock -= beat;
                            beat = ParseNoteDuration();
                            HandleNoteCubes(GetTargetNeckNoteIndex());
                        }
                        // For testing, set voltage of keyboard input
                        SetPlayedNoteVoltage();
                        hitNote = HasPlayerHitNote();

                        // Check to see how close to perfect you are
                        // and gives you a bonus if your close
                        if (hitNote == false)
                        {
                            if (clock > quarterNoteLength / 2)
                            {
                                Debug.Log("WellTimed bonus of two");
                                playerAccuracy = 2;
                            }
                            if (clock > ((quarterNoteLength / 2) + (quarterNoteLength / 4)))
                            {
                                Debug.Log("WellTimed bonus of four");
                                playerAccuracy = 4;
                            }
                        }
                        clock += Time.deltaTime;

                        // Exit back to menu
                        if (Input.GetKeyDown(KeyCode.P))
                        {
                            DestroyNoteCubes();
                            StopCubeNotesCoroutines();
                            Debug.Log("End was called");
                            STATE = "MENU";
                        }
                    }
                    break;

                case "SCORE":
                    Score();
                    break;

                default:
                    // Goes back to menu 
                    STATE = "MENU";
                    break;
            }
        } catch (Exception e)
        {   // Currently throws when the stack runs dry...which is good...kinda
            Debug.Log("State switch-case broke :(");
            Debug.LogException(e, this);
        }
    }

    private void HandleNoteCubes(int targetCubeIndex)
    {
        SpawnCubeNotes(cubeSpawnDistance, targetCubeIndex);
        MoveCubeNotesToNote(targetCubeIndex, (cubeSpawnDistance / beat));
    }

    private IEnumerator Countdown(int timeToCount)
    {
        int count = timeToCount;
        while (count > 0)
        {
            count -= 1;
            yield return new WaitForSeconds(1);
        }
        startMap = true;
    }

    private float ParseNoteDuration()
    {
        float duration = song.currentNote.duration;
        // If d < 0 then its a dotted note
        if (duration > 0)
        {
            switch (duration)
            {
                case 1f : return quarterNoteLength * 4;
                case 2f : return quarterNoteLength * 2;
                case 4f : return quarterNoteLength;
                case 8f : return quarterNoteLength / 2;
                case 16f: return quarterNoteLength / 4;
            }
        }
        // Dotted notes are times that are the
        // sum of their time and half their time
        switch (duration)
        {
            case -1f : return quarterNoteLength + (quarterNoteLength / 2);
            case -2f : return (quarterNoteLength *2) + quarterNoteLength;
            case -4f : return quarterNoteLength + quarterNoteLength / 2;
            case -8f : return (quarterNoteLength / 2) + (quarterNoteLength / 4);
            case -16f : return (quarterNoteLength / 4) + (quarterNoteLength / 8);
        }
        return quarterNoteLength;
    }

    // Testing func, will set voltage of string on keyboard input
    private void SetPlayedNoteVoltage()
    {        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            inputVoltage = Tunnings.voltageFromFret1[0];
            Debug.Log(inputVoltage);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            inputVoltage = Tunnings.voltageFromFret1[1];
            Debug.Log(inputVoltage);
        } 
        else if (Input.GetKeyDown(KeyCode.E))
        {
            inputVoltage = Tunnings.voltageFromFret1[2];
            Debug.Log(inputVoltage);
        } 
        else if (Input.GetKeyDown(KeyCode.R))
        {
            inputVoltage = Tunnings.voltageFromFret1[3];
            Debug.Log(inputVoltage);
        } 
        else if (Input.GetKeyDown(KeyCode.D))
        {
            inputVoltage = Tunnings.voltageFromFret1[4];
            Debug.Log(inputVoltage);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            inputVoltage = Tunnings.voltageFromFret1[5];
            Debug.Log(inputVoltage);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {   
            inputVoltage = 0f;
            Debug.Log(inputVoltage);
        }   
    }



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
    private void MoveCubeNotesToNote(int noteIndex, float cubeSpeed)
    {
        for(int i = 0; i < cubeNotes.Length; i++)
        {
            cubeNotesCoroutines[i] = MoveCube(cubeNotes[i], notes[noteIndex].transform.position, cubeSpeed);
            StartCoroutine(cubeNotesCoroutines[i]);
        }
    }
    private void DestroyNoteCubes()
    {
        // Destroy all cubeNote Cubes to clear array;
        if(cubeNotes.Length > 0)
        {
            for(int i = 0; i < cubeNotes.Length; i++)
            {
                Destroy(cubeNotes[i]);
            }

        }
    }
    private void StopCubeNotesCoroutines()
    {
        // Stops all cube note coroutines
        if (cubeNotesCoroutines.Length > 0)
        {
            for (int i = 0; i < cubeNotesCoroutines.Length; i++)
            {
                if(cubeNotesCoroutines[i] != null)
                {
                    StopCoroutine(cubeNotesCoroutines[i]);
                }
            }

        }
    }
    // TODO NEXT \\
    private int GetPlayedNoteIndex()
    {
        int playedNoteIndex = 0;
        int pin = GetPlayedString();
        int noteIndex = neckHolder.GetStrings()[pin].GetNoteIndex(inputVoltage);

        noteIndex += 1;
        playedNoteIndex = pin * 6;
        playedNoteIndex -= 1;
        playedNoteIndex += noteIndex;
        return playedNoteIndex;
    }

    // Get the index of the current note from the notes prefab array
    private int GetTargetNeckNoteIndex()
    {
        string targetNoteName = song.currentNote.noteName.ToUpper() + song.currentNote.noteOctave;
        for(int i = 0; i < notes.Length; i++)
        {
            if (notes[i].name == targetNoteName)
            {
                Debug.Log("This should be called");
                return i;                
            }
        }
        Debug.Log("THIS SHOULDNT BE CALLED");
        return -1;
    }
    private bool HasPlayerHitNote()
    {
        int targetNote = GetTargetNeckNoteIndex();
        int playedNote = GetPlayedNoteIndex();
        if (targetNote == playedNote)
        {
            return true;
        }
        return false;
    }
 
    // TODO \\
    public void Menu()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            STATE = "MAPPED_SONG";
        }
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
        song = new MappedSong(_name);
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

    // FUCK IT SPAWNS IT ALL AS A LEFT HANDED GUITAR SHIT
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