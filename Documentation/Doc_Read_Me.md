# Documentation

## How the game works
The game works with an electronic guitar interface into a game in unity. The game has two modes, a free mode and a rhythm mode. The free mode takes the note your playing and generates the note. The other main mode, rhythm, shows you notes to play along to a song. You play the notes along with the song and every time you hit a right note, you get points. If you score below a certain score you lose. You should try to get the highest note possible.
## How the game works Technically
Songs are made into "maps". These maps hold all the needed data for the songs, such as the file path for the mp3, the position of notes-to-play on the neck, the name of notes ext...Songs are written using the static song template class. 
```c#
    public string songName;
    public int noteCount;
    public int timeSigTop;
    public int timeSigBot;
    public int[] nDur;
    public int[] sIndex;
    public int[] nIndex;
    public string sFilePath;
```
While the map template is made before the build, the map is generated run-time and loaded into the game when you start. Notes are popped off a stack in the map and then handled by the game controller script to be played in the game. This map note data is stored as a struct. 
```c#
    public struct MappedNote
    {
        public float duration { get; set; }
        public int sIndex { get; set; }
        public int nIndex { get; set; }
    }
```
Cubes move toward the required note to show the timming for the player, they have until the cubes hit the note to ply the correct note. The better timed they play the note the more points they get. 
```c#
    private IEnumerator MoveCube(GameObject cube, Vector3 destination, float speed)
    {

        while (cube.transform.position != destination)
        {
            cube.transform.position = Vector3.MoveTowards(cube.transform.position, destination, speed * Time.deltaTime);
            yield return null;
        }
    }
```
The notes that are seen in the game are prefabs that are instantiated and loaded during run time. They are spawned, along with the strings, and given a Note object. This note object is instantiated from the Note class and holds information for that note according to the tunning of the virtual guitar. These notes are generated from the class NeckHolder, which handles the loading and instantiating of string objects and note objects. String objects have 6 Note objects each and each note is related to a "note" prefab on the game's guitar neck. The string class calls various methods from each Note object to initialize the note correctly. Each correctly initialized note contains data about itself, such as which octave it is, the note's name, the frequency of the note, its voltage and its voltage range. This is the voltage that is used to determine whether the player has played the correct note on the guitar controller or not, and is accessed from the game controller when calling the method isNote(). 
```c#
    public bool IsNote(float inputVoltage)
    {
        if (inputVoltage >= minVoltage && inputVoltage <= maxVoltage)
        {
            return true;
        }
        return false;
    }
```
A state machine handles moving the game between states and songs. 
```c#
    enum STATE
    {
        MENU, FREE_PLAY, MAPPED_SONG, SCORE
    }
```
When the mapped song is playing, it plays to a tempo, this is easy changeable from the inspector if needed to change difficulty, but is predefined to match the map's tempo. The timing of the note's that the user needs to play are all relative to the tempo, since the timings of quarter notes, half notes, dotted notes ext... are all calculated using the temp. Free play mode works by taking the input from the guitar and generating the note, from the note's frequency. The frequency is relative to the octave and was scrapped using a python script from [michigan tech.edu](https://pages.mtu.edu/~suits/notefreqs.html) under a free use licence.

The game determines whether the user has played a correct note by taking an analogue reading an integer value between 0-1024 from the Arduino and finding the product of the reading with the scalar of (5 / 1023) to map the values between 0 - 5v . If this is between a predetermined range for the required note, then it registers as a played note. This is why the Note class has a perfect voltage attribute (the target voltage) but also a min-max voltage (which is calculated by += a given percentage). The problem with this however is that the pins constantly give out a voltage reading, regardless or not to whether there is one. This noise can be handled by calculating the average reading and comparing that average to the min-max Note voltages. This is done within the Arduino. Pre-made is Udruino is a sketch that defines methods for unity to call. I edited this sketch so that when a script in unity makes a call to read an analogue pin, it's actually reading the average of several readings. 
```c++
if (pinToRead != -1)
	// Innit cummulative value and average value
	value = 0;
	values = 0;
	// Add to the cummulative value
	for(int i = 0; i < 15; i++)
	{
		value = analogRead(pinToRead);
		values += value;
	}
	// Calculate the mean and print it
	value = values / 15 ;
	printValue(pinToRead, value);
}
```
This is down in Arduino not unity as the tick-rate of Arduino is significantly fast enough to do so without causing delays in the game.

## How Controller Works

The controller is essentially a *gutted* potentiometer. There's a potential divider circuit with 7 10k resistors inside the body of the guitar, with a 5v in from the Arduino. Along the neck, on the first 6 frets, there's copper tape spread across the neck. Connected to this copper tape is the wiring which at the other end is connected to the potential divider circuit, with the first fret being between resistor 1 and 2, fret 2 being between resistor 2 and 3 and so on. At the bridge of the guitar, connected the strings in a wire that leads to an analogue pin in the Arduino. In standard tuning, Pin A0 is connected to the high e string, A1 is connected to the b string and so on. When the player plays a note, they press down on a string at a fret, this creates a contract between the string and the copper tape, connecting the analogue pins to the potential divider. The Arduino constantly reads and averages out the pin input, but now it's sending a constant average, as the pin will be roughly the stepped-down voltage between the resistor.


<img src="Photos/FinishedProjectFront.jpg" alt="Finished Controller Front" width="200"/>  <img src="Photos/FinishedProjectWiring.jpg" alt="Finished Controller Wiring" width="200"/>  <img src="Photos/GameScreenshot.png" alt="Game Screenshot" width="300"/>  <img src="Photos/Screenshot of game with score working.jpg" alt="Game Screenshot" width="300"/>
