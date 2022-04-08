using System;
using System.Collections;
using UnityEngine;
using Uduino;
using UnityEditor.UI;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    #region variables

    // Spawning Variables
    public GameObject stringSpawner;
    public GameObject notePrefab;
    public GameObject stringPrefab;
    public GameObject noteSpawner;
    private GameObject[] strings = new GameObject[36];
    private GameObject[] notes = new GameObject[36];
    private NeckHolder neckHolder;
    public float stringGapScalar;
    public float noteGapScalar;    


    // State machine
    enum STATE
    {
        MENU, FREE_PLAY, MAPPED_SONG, SCORE
    }
    private STATE activeState = STATE.MENU;
    
    // Mapped Song to be played
    private MappedSong song = null;
    public AudioSource audioSource;
    private double gain = 0;

    // Moveing Note Data
    public GameObject cubeNote;
    private GameObject[] cubeNotes = new GameObject[4];
    public int cubeSpawnDistance = 10;
    private IEnumerator[] cubeNotesCoroutines = new IEnumerator[4];

    // Song Game Loop Data
    private float clock;
    private float beat;
    private float quarterNoteLength;
    private int noteCount;
    public int tempo = 60;

    // Bool Flags for game flow
    private bool startMap = false;
    private bool hitNote = false;
    private bool isPlayingMap = false;

    // Uduino Data
    private UduinoManager manager;
    private float scalar;
    public float[] analogValues = new float[6];

    // Score vars
    private int playerScore = 0;
    private int playerAccuracy = 0;
    // Score UI values
    private string topScoreText;
    public Text topScore;
    private string currentScoreText;
    public Text currentScore;

    #endregion
    private void Awake()
    {
        // Innits all strings and notes for standard tunnings
        neckHolder = new NeckHolder(Tunnings.standardTunning);
        // Spawn in the string and note prefabs
        SpawnStrings(stringSpawner.transform.position);
        SpawnNotes(noteSpawner.transform.position);

        // Innit for the Uduino connection
        manager = UduinoManager.Instance;
        manager.pinMode(AnalogPin.A0, PinMode.Input);
        manager.pinMode(AnalogPin.A1, PinMode.Input);
        manager.pinMode(AnalogPin.A2, PinMode.Input);
        manager.pinMode(AnalogPin.A3, PinMode.Input);
        manager.pinMode(AnalogPin.A4, PinMode.Input);
        manager.pinMode(AnalogPin.A5, PinMode.Input);
        // Analog pin input outputs a range of ints
        // from 0 - 1023 (2^10 values). Devide by the
        // max input voltage to get a scalar ratio and
        // then multiply that by the actual voltage
        // reading to find the actual voltage reading
        // at the pin where 0.0 =< v <= 5.0
        scalar = (5f / 1023f);

    }

    private void Start()
    {
        // Gets Arduino values as the first few values can be buggy as its connecting 
        for (int i = 0; i < 10; i++)
        {
            GetValues();
        }
    }

    //[Obsolete]
    private void Update()
    {
        try
        {
            switch (activeState)
            {
                case STATE.MENU:
                    Menus();
                    break;

                case STATE.FREE_PLAY:
                    FreePlay();                    
                    break;

                case STATE.MAPPED_SONG:
                    // Instantiates mapped song and game mode
                    // Runs everytime the map state is laoded
                    // but only once at the start
                    if (isPlayingMap == false)
                    {
                        startMap = true;
                        LoadMap("Cannon");
                        // Innit score
                        SetScore(0, Int32.Parse(topScore.text));
                        quarterNoteLength = 60f / tempo;
                        beat = quarterNoteLength;

                        // Set clock as beat so that the next
                        // condition is true and the first note
                        // is immediately ran
                        clock = beat;
                        isPlayingMap = true;
                        noteCount = 0;
                        playerAccuracy = 0;
                        playerScore = 0;
                        audioSource.clip = Resources.Load<AudioClip>(song.sFilePath);
                        audioSource.Play();
                    }

                    // Starts game map
                    if (startMap)
                    {
                        // Plays next note
                        if (clock >= beat)
                        {
                            // Increameants score only if the player hit the note                        
                            if (hitNote)
                            {
                                playerScore += 1 + playerAccuracy;
                                UpdateCurrentScore();
                            }
                            // Reset all values for individual note
                            playerAccuracy = 0;
                            hitNote = false;
                            ResetCubeMaterial();
                            DestroyNoteCubes();
                            StopCubeNotesCoroutines();
                            // If the song is completed, returns to the menu state
                            noteCount += 1;
                            if (noteCount == song.GetLengthOfMap())
                            {
                                // Change state to leave map
                                activeState = STATE.SCORE;
                                isPlayingMap = false;
                                break;
                            }
                            // Get the next note, reset the clock and the reset the noteCubes
                            song.PlayNote();
                            clock -= beat;
                            beat = ParseNoteDuration();
                            HandleNoteCubes(GetTargetNeckNoteIndex(song.currentNote.sIndex, 5-song.currentNote.nIndex));
                        }               
                        HanderArduinoInput();
                        if (!hitNote)
                        {
                            hitNote = HasPlayerHitNote();
                        }

                        // Check to see how close to perfect you are
                        // and gives you a bonus if your close
                        if (hitNote == false)
                        {
                            if (clock > quarterNoteLength / 2)
                            {
                                playerAccuracy = 2;
                            }
                            if (clock > ((quarterNoteLength / 2) + (quarterNoteLength / 4)))
                            {
                                playerAccuracy = 4;
                            }
                        }
                        clock += Time.deltaTime;

                        // Exit back to menu
                        if (Input.GetKeyDown(KeyCode.P))
                        {
                            ResetCubeMaterial();
                            DestroyNoteCubes();
                            StopCubeNotesCoroutines();
                            Debug.Log("End was called");
                            activeState = STATE.MENU;
                            isPlayingMap = false;
                            audioSource.Stop();
                        }
                    }
                    break;

                case STATE.SCORE:
                    Score();
                    break;

                default:
                    // Goes back to menu 
                    audioSource.Stop();
                    activeState = STATE.MENU;
                    break;
            }
        }
        // Catches any error in the state machine
        catch (Exception e)
        {
            Debug.Log("State switch-case broke :(");
            Debug.LogException(e, this);
        }
    }

    /// <summary>
    /// Gets the averaged values of the analogue 
    /// pins and sets them to analogValues
    /// </summary>
    public void HanderArduinoInput()
    {
        analogValues = GetValues();
    }

    /// <summary>
    /// Gets the values from the aurduino pins and maps them
    /// to their true voltage
    /// </summary>
    /// <returns>float array of voltages from 0 - 5 where index == pin number</returns>
    private float[] GetValues()
    {
        float[] temp = new float[6];

        int sensorValueA0 = manager.analogRead(AnalogPin.A0);
        float voltageA0 = sensorValueA0 * scalar;
        temp[0] = voltageA0;

        int sensorValueA1 = manager.analogRead(AnalogPin.A1);
        float voltageA1 = sensorValueA1 * scalar;
        temp[1] = voltageA1;

        int sensorValueA2 = manager.analogRead(AnalogPin.A2);
        float voltageA2 = sensorValueA2 * scalar;
        temp[2] = voltageA2;        
        int sensorValueA3 = manager.analogRead(AnalogPin.A3);
        float voltageA3 = sensorValueA3 * scalar;
        temp[3] = voltageA3;

        int sensorValueA4 = manager.analogRead(AnalogPin.A4);
        float voltageA4 = sensorValueA4 * scalar;
        temp[4] = voltageA4;

        int sensorValueA5 = manager.analogRead(AnalogPin.A5);
        float voltageA5 = sensorValueA5 * scalar;
        temp[5] = voltageA5;

        return temp;
    }



    /// <summary>
    /// Handles the spawning and movemeant of the note cubes 
    /// to show the player the timings to hit the target note
    /// </summary>
    /// <param name="targetCubeIndex">Index of the target note for the player on the board</param>
    private void HandleNoteCubes(int targetCubeIndex)
    {
        ChangeMaterialTo(notes[targetCubeIndex], "Materials/Notes/RightNote");
        SpawnCubeNotes(cubeSpawnDistance, targetCubeIndex);
        MoveCubeNotesToNote(targetCubeIndex, (cubeSpawnDistance / beat));
    }

    /// <summary>
    /// Calculates length in seconds of the given note
    /// relitive to its duration (4th, 16th ext...)
    /// </summary>
    /// <returns>Float note duration</returns>
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

    /// <summary>
    /// Sets the top score and current score text as the args
    /// </summary>
    /// <param name="currentScoreValue">The players current score</param>
    /// <param name="topScoreValue">The players top score so far</param>
    private void SetScore(int currentScoreValue, int topScoreValue)
    {
        currentScoreText = currentScoreValue.ToString();
        topScoreText = topScoreValue.ToString();

        currentScore.text = currentScoreText;
        topScore.text = topScoreText;
    }

    /// <summary>
    /// Spawns 4 cube notes in at the given distance around the target note
    /// </summary>
    /// <param name="spawnDistance">Distance for the cubes to be spawned away from the target note</param>
    /// <param name="noteIndex">Target note index for the cubes to be spawned around</param>
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

    /// <summary>
    /// Moves the note cubes towards the target note
    /// </summary>
    /// <param name="noteIndex">Target note index</param>
    /// <param name="cubeSpeed">The speed atwhich the cubes will move towards the note</param>
    private void MoveCubeNotesToNote(int noteIndex, float cubeSpeed)
    {
        for(int i = 0; i < cubeNotes.Length; i++)
        {
            cubeNotesCoroutines[i] = MoveCube(cubeNotes[i], notes[noteIndex].transform.position, cubeSpeed);
            StartCoroutine(cubeNotesCoroutines[i]);
        }
    }

    /// <summary>
    /// Resets the cube material to its default
    /// </summary>
    private void ResetCubeMaterial()
    {
        ChangeMaterialTo(notes[GetTargetNeckNoteIndex(song.currentNote.sIndex, 5 - song.currentNote.nIndex)], "Materials/Notes/NoteDefault");
    }

    private IEnumerator ResetACubeMaterialAfterTime(int waitTime, int cubeIndex)
    {
        yield return new WaitForSeconds(waitTime);
        ChangeMaterialTo(notes[cubeIndex], "Materials/Notes/NoteDefault");
        yield break;
    }

    /// <summary>
    /// Destroys all currently spawned note cubes inside the cubeNotes array
    /// </summary>
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

    /// <summary>
    /// Stops all cube note coroutines
    /// </summary>
    private void StopCubeNotesCoroutines()
    {
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

    /// <summary>
    /// Gets the target note index to be played in the notes prefab
    /// </summary>
    /// <param name="sIndex">The string index 0-5</param>
    /// <param name="nIndex">The note index 0-5</param>
    /// <returns>Target ntoe index</returns>
    private int GetTargetNeckNoteIndex(int sIndex, int nIndex)
    {
        int playedNoteIndex = 0;
        // Code here loops through the 0-35 arrary
        // in groups of 6
        nIndex += 1;
        playedNoteIndex = sIndex * 6;
        playedNoteIndex -= 1;
        playedNoteIndex += nIndex;
        return playedNoteIndex;
    }

    /// <summary>
    /// Checks to see if the player has hit the target note
    /// </summary>
    /// <returns>Bool has or hasn't hit</returns>
    private bool HasPlayerHitNote()
    {
        // Target note and played note
        int targetNote = GetTargetNeckNoteIndex(song.currentNote.sIndex, 5- song.currentNote.nIndex);
        int noteIndex = -1;
        int pin = 0;
        int playedNote;
        for (int i = 0; i < 6; i++)
        {
            int tempIndex = neckHolder.GetStrings()[i].GetNoteIndex(analogValues[i]);
            if (tempIndex != -1)
            {
                noteIndex = tempIndex;
                pin = i;
                if( noteIndex != -1)
                {
                    playedNote = GetTargetNeckNoteIndex(pin,(5 - noteIndex));
                }
                else
                {
                    return false;
                }
        
                if (targetNote == playedNote)
                {            
                    Debug.Log("Correct note played at index of " + playedNote + " and of name " + notes[playedNote].name + " with a voltage of " + analogValues[i]);
                    return true;
                }
                break;
            }
        }
        return false;
    }
 
    /// <summary>
    /// Loads the menu scene
    /// </summary>
    public void Menus()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            activeState = STATE.MAPPED_SONG;
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("f pressed");
            activeState = STATE.FREE_PLAY;
        }
    }

    /// <summary>
    /// Updates the ui showing the current score to the players current score
    /// </summary>
    private void UpdateCurrentScore()
    {        
        currentScoreText = playerScore.ToString();
        currentScore.text = currentScoreText;
    }

    /// <summary>
    /// Handles the score scene
    /// </summary>
    public void Score()
    {
        Debug.Log("SCORE: Score is " + playerScore);
        int currentTopScore = Int32.Parse(topScore.text);
        if (currentTopScore < playerScore)
        {
            SetScore(playerScore, playerScore);
        }
        else
        {
            SetScore(playerScore, currentTopScore);
        }
        activeState = STATE.MENU;
    }

    /// <summary>
    /// Loads the map data from the map name into the 
    /// MappedSong object, song
    /// </summary>
    /// <param name="_name">Name of the map</param>
    private void LoadMap(string _name)
    {
        song = new MappedSong(_name);
    }

    
    /// <summary>
    /// Plays FreePlay mode
    /// </summary>
    public void FreePlay()
    {
        Debug.Log("In Free play");
        for (int i = 0; i < 6; i++)
        {
            int tempIndex = neckHolder.GetStrings()[i].GetNoteIndex(analogValues[i]);
            // If there is a played note, play it
            if (tempIndex != -1)
            {
                int notesIndex = GetTargetNeckNoteIndex(i, tempIndex);
                Note playedNote = neckHolder.GetStrings()[i].GetNote(analogValues[i]);
                Debug.Log("Played Note's name is " + playedNote.GetName());
                // Plays the generated note
                PlayNote(playedNote);
                // Change the material of the playted note to a new material               
                ChangeMaterialTo(notes[notesIndex], "Materials/Notes/PlayedNote");
                // Start the coroutine to change it back to its default after 1 second
                StartCoroutine(ResetACubeMaterialAfterTime(1, notesIndex));

            }
        }
        // Exit back to menu
        if (Input.GetKeyDown(KeyCode.P))
        {
            activeState = STATE.MENU;
        }        
    }

    /// <summary>
    /// Moves the cube towards the target node every frame,
    /// until its has reached it's location   
    /// </summary>
    /// <param name="cube">Note cube to move</param>
    /// <param name="destination">Location to move cube towards</param>
    /// <param name="speed">Rate to move cube at</param>
    /// <returns></returns>
    private IEnumerator MoveCube(GameObject cube, Vector3 destination, float speed)
    {

        while (cube.transform.position != destination)
        {
            cube.transform.position = Vector3.MoveTowards(cube.transform.position, destination, speed * Time.deltaTime);
            yield return null;
        }
    }

    /// <summary>
    /// Played the given Note object and turns the others off
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

    /// <summary>
    /// Changes the material of a given gameobject to the material 
    /// located at the given file path within resources
    /// </summary>
    /// <param name="g">Gameobject to change the material of</param>
    /// <param name="path">File path starting from within resources</param>
    private void ChangeMaterialTo(GameObject g, string path)
    {
        g.GetComponent<Renderer>().material = Resources.Load(path, typeof(Material)) as Material;
    }

    /// <summary>
    /// Spawns strings on the backing and stores them in the strings[6] array
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
    /// Spawns notes along the strings and stores the reference in notes[36] array
    /// </summary>
    void SpawnNotes(Vector3 spawnPosition)
    {        
        int noteCounter = 0;
        for(int i = 0; i < 6; i++)
        {    
            for (int j = 0; j < 6; j++)
            {
                // Create new note and create its location/rotation
                Note currentNote = neckHolder.GetStrings()[i].notes[5-j];
                Quaternion q = new Quaternion(0, 0, 0, 1);
                Vector3 spawn = new Vector3(spawnPosition.x + i * stringGapScalar, spawnPosition.y, spawnPosition.z + j * noteGapScalar);

                // Instantiate note prefab and add the created note to the prefab
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