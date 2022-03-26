using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SongToMapData
{
    // Dictionary holding all song data for mapping
    // Creates a SongTemplate object per song
    private static readonly Dictionary<string, SongTemplate> songsDict = new Dictionary<string, SongTemplate>
    {
        { "AtDoomsGate" , 
            new SongTemplate(
                "AtDoomsGate",
                6,
                new string[] { "e", "e", "e", "e", "e", "d" },
                new int[] { 2, 2, 3, 2, 2, 3 },
                new int[] { 8, 8, 8, 8, 8, 8 }
                )
        },
        { "CanonInD" ,
            new SongTemplate(
                "CanonInD",
                6,
                new string[] { "e", "e", "e", "e", "e", "d" },
                new int[] { 2, 2, 3, 2, 2, 3 },
                new int[] { 8, 8, 8, 8, 8, 8 }
                )
        },
        { "Test" ,
            new SongTemplate(
                "Test",
                6,
                new string[] { "e", "e", "e", "e", "e", "d" },
                new int[] { 4, 4, 4, 4, 4, 4 },
                new int[] { 8, 8, 8, 8, 8, 8 }
                )
        }
    };
    public static SongTemplate GetSongToMapData(string name)
    {

        foreach (KeyValuePair<string, SongTemplate> ele in songsDict)
        {
            if(ele.Key == name)
            {                                          
                Debug.Log("We found the data " + ele.Value.songName + ele.Value.noteCount);
                return ele.Value;
            }
        }
        return null;
    }
}


// Name...Octave...Duration 
// 4 is a quter note 8 is an eighteenth 16 is a sixteenth ext...
// Negitive numbers stand for dotted notes, so -4 means a
// quarter plus an eighteenth
//map.MapNote("e", 2, 8);
//map.MapNote("e", 2, 8);
//map.MapNote("e", 3, 8);
//map.MapNote("e", 2, 8);
//map.MapNote("e", 2, 8);
//map.MapNote("d", 3, 8);
//map.MapNote("e", 2, 8);
//map.MapNote("e", 2, 8);
//map.MapNote("c", 3, 8);
//map.MapNote("e", 2, 8);
//map.MapNote("e", 2, 8);
//map.MapNote("a#", 2, 8);
//map.MapNote("e", 2, 8);
//map.MapNote("e", 2, 8);
//map.MapNote("b", 2, 8);
//map.MapNote("c", 3, 8);
//map.MapNote("e", 2, 8);
//map.MapNote("e", 2, 8);
//map.MapNote("e", 3, 8);
//map.MapNote("e", 2, 8);
//map.MapNote("e", 2, 8);
//map.MapNote("d", 3, 8);
//map.MapNote("e", 2, 8);
//map.MapNote("e", 2, 8);
//map.MapNote("c", 3, 8);
//map.MapNote("e", 2, 8);
//map.MapNote("e", 2, 8);
//map.MapNote("a#", 2, -2);
// 28 Notes but thers only 27 (im missing one ahhhhh)^^