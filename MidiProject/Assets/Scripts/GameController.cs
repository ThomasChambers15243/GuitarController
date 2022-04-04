using System;
using System.Collections;
using UnityEngine;
using Uduino;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    #region variables
    // Spawning Variables
    public GameObject stringSpawner;
    public GameObject notePrefab;
    public GameObject stringPrefab;
    public GameObject noteSpawner;
    // Gaps between strings and notes
    public float stringGapScalar;
    public float noteGapScalar;
    // WILL BE VOLTAGE FROM GUITAR BUT FOR NOW ITS FROM ME
    public float testingInputVoltage;
    private float inputVoltage = 0;    

    // State machine
    enum STATE
    {
        MENU, FREE_PLAY, MAPPED_SONG, SCORE
    }
    private STATE activeState = STATE.MENU;
    
    // Mapped Song to be played
    private MappedSong song = null;
    private string[] maps = new string[] { "AtDoomsGate", "CanonInD", "Test", "Test1" };

    // All note and string perfabs
    private NeckHolder neckHolder;
    private GameObject[] strings = new GameObject[36];
    private GameObject[] notes = new GameObject[36];
    private double gain = 0;

    // Moveing Note Signals
    public GameObject cubeNote;
    private GameObject[] cubeNotes = new GameObject[4];

    // Song Game Loop Data
    // Distance cubes spawns from the target note 
    // before they move in
    private float clock;
    private float beat;
    private float quarterNoteLength;
    private int noteCount;
    public int cubeSpawnDistance = 10;
    public int tempo = 60;
    private bool startMap = false;
    private bool hitNote = false;
    private bool isPlayingMap = false;
    private IEnumerator[] cubeNotesCoroutines = new IEnumerator[4];
    // Score vars
    private int playerScore = 0;
    private int playerAccuracy = 0;

    // Uduino Data
    private UduinoManager manager;
    private float scalar;
    public float[] analogValues = new float[6];
    private float[] cumValues = new float[6];
    private bool valuesFound = true;
    private int readingCounter;

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
                    Menu();
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
                        //LoadMap("EasyTest");
                        //LoadMap("GMajor");
                        LoadMap("EasyTest2");
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
                                song.score += 1 + playerAccuracy;
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
                                playerScore = song.score;
                                break;
                            }
                            // Get the next note, reset the clock and the reset the noteCubes
                            song.PlayNote();
                            clock -= beat;
                            beat = ParseNoteDuration();
                            HandleNoteCubes(GetTargetNeckNoteIndex(song.currentNote.sIndex, 5-song.currentNote.nIndex));
                        }
                        // For testing, set voltage of keyboard input
                        //SetPlayedNoteVoltage();                       
                        HanderArduinoInput();
                        hitNote = HasPlayerHitNote();

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
                        }
                    }
                    break;
                // TODO \\
                case STATE.SCORE:
                    Score();
                    break;

                default:
                    // Goes back to menu 
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


    public void HanderArduinoInput()
    {
        if (valuesFound)
        {
            valuesFound = false;
            readingCounter = 0;
            analogValues = new float[6] { 0, 0, 0, 0, 0, 0 };
            cumValues = new float[6] { 0, 0, 0, 0, 0, 0 };
        }
        while (readingCounter < 5)
        {
            float[] tempV = new float[6];
            tempV = GetValues();
            for (int i = 0; i < 6; i++)
            {
                cumValues[i] += tempV[i];
            }
            readingCounter += 1;
        }
        valuesFound = true;

        if (!Array.Exists(cumValues, x => x == 0f))
        {
            for (int i = 0; i < 6; i++)
            {
                analogValues[i] = cumValues[i] / readingCounter;
                //Debug.Log(analogValues[i]);
            }
        }
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

        //for (int i = 0; i < 6; i++)
        //{
        //    Debug.Log(temp[i]);
        //}

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
    /// Countdown for the map to start
    /// </summary>
    /// <param name="timeToCount">Length of countdown</param>
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

    // Testing func, will set voltage of string on keyboard input
    private void SetPlayedNoteVoltage()
    {        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            inputVoltage = Tunnings.voltageFromFret1[0];
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            inputVoltage = Tunnings.voltageFromFret1[1];
        } 
        else if (Input.GetKeyDown(KeyCode.E))
        {
            inputVoltage = Tunnings.voltageFromFret1[2];
        } 
        else if (Input.GetKeyDown(KeyCode.R))
        {
            inputVoltage = Tunnings.voltageFromFret1[3];
        } 
        else if (Input.GetKeyDown(KeyCode.D))
        {
            inputVoltage = Tunnings.voltageFromFret1[4];
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            inputVoltage = Tunnings.voltageFromFret1[5];
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {   
            inputVoltage = 0f;
        }   
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

    // ???????????????????
    /// <summary>
    /// Resets the cube material to its default
    /// </summary>
    private void ResetCubeMaterial()
    {
        ChangeMaterialTo(notes[GetTargetNeckNoteIndex(song.currentNote.sIndex, 5 - song.currentNote.nIndex)], "Materials/Notes/NoteDefault");
    }

    // Destroys all currently spawned note cubes inside the cubeNotes array
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
            // 0 should be i
            int tempIndex = neckHolder.GetStrings()[i].GetNoteIndex(analogValues[i]);
            //Debug.Log(analogValues[i]);
            if (tempIndex != -1)
            {
                Debug.Log("Note found at: " + analogValues[i]);
                noteIndex = tempIndex;
                pin = i;
                break;
            }
        }
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
            Debug.Log("Index is " + playedNote + " of name " + notes[playedNote].name);
            return true;
        }
        return false;
    }
 
    // TODO \\
    /// <summary>
    /// Loads the menu scene
    /// </summary>
    public void Menu()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            activeState = STATE.MAPPED_SONG;
        }
        // Open's game menu and changes state to either
        // Free play, Mapped Song or Score
    }

    // TODO \\
    // Loads the Score scene
    public void Score()
    {
        // Will display the score board
        Debug.Log("SCORE: Score is " + playerScore);
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

    // TODO - OUT OF DATE, DON'T TRUST \\
    /// <summary>
    /// Plays FreePlay mode
    /// </summary>
    public void FreePlay()
    {
        int pin = GetPlayedString();
        Note playedNote = neckHolder.GetStrings()[pin].GetNote(inputVoltage);
        PlayNote(playedNote);
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

    // TODO currently works of note name, not index,
    // probally a good idea to have too methods,
    // one that works with raw notes[36] index and 
    // one that works with names, like this one does
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

    // TODO This may be absolutly pointless idk yet
    /// <summary>
    /// Returns the index of the string that was played relitive to the input pin
    /// Pin A0 == string 0
    /// </summary>
    /// <returns>Index of string played</returns>
    private int GetPlayedString()
    {
        return 0;
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
                Note currentNote = neckHolder.GetStrings()[i].notes[5-j];
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

//ARCHIVE cus im too scared to delete this code right now
//// TODO NEXT \\
//private int GetPlayedNoteIndex()
//{
//    int playedNoteIndex = 0;
//    int pin = GetPlayedString();
//    // 5 minus for now as notes are all backwards...blame past Tom it wasn't me
//    int noteIndex = 5-(neckHolder.GetStrings()[pin].GetNoteIndex(inputVoltage));

//    noteIndex += 1;
//    playedNoteIndex = pin * 6;
//    playedNoteIndex -= 1;
//    playedNoteIndex += noteIndex;
//    return playedNoteIndex;
//}

//// Get the index of the current note from the notes prefab array
//private int GetTargetNeckNoteIndexOLD()
//{
//    string targetNoteName = song.currentNote.noteName.ToUpper() + song.currentNote.noteOctave;
//    for(int i = 0; i < notes.Length; i++)
//    {
//        if (notes[i].name == targetNoteName)
//        {
//            Debug.Log("This should be called");
//            return i;                
//        }
//    }
//    Debug.Log("THIS SHOULDNT BE CALLED");
//    return -1;
//}
// int targetNote = GetTargetNeckNoteIndexOLD();
// int playedNote = GetPlayedNoteIndex();