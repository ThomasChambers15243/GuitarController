using System.Collections.Generic;
using UnityEngine;

public static class SongToMapData
{
    // Dictionary holding all song data for mapping
    // Creates a SongTemplate object per song
    private static readonly Dictionary<string, SongTemplate> songsDict = new Dictionary<string, SongTemplate>
    {
        {
            // Extra note at the start so timmings are shifted
            // Extra note at the end too
            "GMajor",
            new SongTemplate(
                "Gmajor",
                10,
                4,
                4,
                new string[] {"g","g","a","b","c","d","e","f#","g","g"}, // 10 notes
                new int[] {2,2,2,2,3,3,3,3,3,3}, // 10 octaves
                new int[] {4,4,4,4,4,4,4,4,4,4}, // 10 durations
                new int[] {5,5,5,4,4,4,3,3,3,3}, // 10 strings
                new int[] {2,2,4,1,2,4,1,3,4,4},  // 10 notes
                ""
                )
        },
        {
            "EasyTest",
            new SongTemplate(
                "EasyTest",
                12,
                4,
                4,
                new string[] {"n","n","n","n","n","n","n","n","n","n","n","n","n","n", }, // 14 notes
                new int[] {2,2,2,2,2,2,2,2,2,2,2,2,2,2},   // 14 octaves
                new int[] {4,4,4,4,4,4,4,4,4,4,4,4,4,4}, // 14 durations
                new int[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0},   // 14 strings
                new int[] {0,1,2,3,4,5,4,3,2,1,0,1,2,3},    // 14 notes
                "Songs/Mine/Mine Medium (100bpm)"
                )
        }
    };

    // Template for copy&paste
    //new SongTemplate(
    //            "EasyTest",
    //            12,
    //            4,
    //            4,
    //            new string[] { }, // 
    //            new int[] { }, // 
    //            new int[] { }, // 
    //            new int[] { }, // 
    //            new int[] { }, // 
    //            )

    /// <summary>
    /// Gets the song data for the given map name
    /// </summary>
    /// <param name="name">Name of map</param>
    /// <returns>Song data as SongTemplate</returns>
    public static SongTemplate GetSongToMapData(string name)
    {

        foreach (KeyValuePair<string, SongTemplate> ele in songsDict)
        {
            if(ele.Key == name)
            {                                          
                return ele.Value;
            }
        }
        return null;
    }
}

// Archive 
//{
//    "AtDoomsGate" ,
//    new SongTemplate(
//        "AtDoomsGate",
//        6,
//        4,
//        4,
//        new string[] { "e", "e", "e", "e", "e", "d" },
//        new int[] { 2, 2, 3, 2, 2, 3 },
//        new int[] { 8, 8, 8, 8, 8, 8 }
//        )
//},
//{
//    "CanonInD" ,
//    new SongTemplate(
//        "CanonInD",
//        6,
//        4,
//        4,
//        new string[] { "e", "e", "e", "e", "e", "d" },
//        new int[] { 2, 2, 3, 2, 2, 3 },
//        new int[] { 8, 8, 8, 8, 8, 8 }
//        )
//},