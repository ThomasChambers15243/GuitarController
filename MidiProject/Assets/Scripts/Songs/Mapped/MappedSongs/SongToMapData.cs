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
                4,
                4,
                new string[] { "e", "e", "e", "e", "e", "d" },
                new int[] { 2, 2, 3, 2, 2, 3 },
                new int[] { 8, 8, 8, 8, 8, 8 }
                )
        },
        { "CanonInD" ,
            new SongTemplate(
                "CanonInD",
                6,
                4,
                4,
                new string[] { "e", "e", "e", "e", "e", "d" },
                new int[] { 2, 2, 3, 2, 2, 3 },
                new int[] { 8, 8, 8, 8, 8, 8 }
                )
        },
        { "Test" ,
            new SongTemplate(
                "Test",// Name
                9,// Total number of notes
                4,// 4/4 time
                4,
                new string[] { "e", "f", "e", "f", "e", "f" ,"f#","g","a",},
                new int[] { 4, 4, 4, 4, 4, 4, 4, 4, 5 }, // Octave
                new int[] { 4, 4, 4, 4, 4, 4, 4,4 ,2 } // Duration
                )
            // cube target is the previous timming but the current notes location
            // the game starts when the player plays the first note
        },
        { "Test1" ,
            new SongTemplate(
                "Test1",
                6,
                4,
                4,
                new string[] { "e", "e", "e", "e", "e", "d" },
                new int[] { 4, 4, 4, 4, 4, 3 },
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