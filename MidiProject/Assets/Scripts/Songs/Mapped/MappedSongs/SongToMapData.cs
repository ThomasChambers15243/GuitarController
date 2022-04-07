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
                new int[] {4,4,4,4,4,4,4,4,4,4}, // 10 durations
                new int[] {5,5,5,4,4,4,3,3,3,3}, // 10 strings
                new int[] {2,2,4,1,2,4,1,3,4,4},  // 10 notes
                ""
                )
        },
        {
            "Mine Meduim",
            new SongTemplate(
            "Mine Meduim", // Song Name
            67, // Number of Notes
            4, // Time Sig
            4, // Time sig
            new int[] { 8,8,8,8,8,8,8,8,  8,8,8,8,8,8,8,8,  8,8,8,8,8,8,8,8,  8,8,8,8,8,8,8,8,  8,8,8,8,8,8,8,8,    8,8,8,8,8,8,8,8,
                        8,8,8,8,8,8,8,8,  16,16,16,16,16,16,16,16,  16,16,16}, // duration
            new int[] { 0,1,2,0,1,2,0,1,  0,1,2,0,1,2,0,1,  0,1,2,0,1,2,0,1,  0,1,2,0,1,2,0,1,  1,2,3,2,  1,2,3,2,  1,2,3,2,  1,2,3,2,
                       // Cmajor                                                                Fmajor              Fm                
            //Cmajor            Cmajor Sweep                                                                
            0,1,2,1,  0,1,2,1,  0,0,0,1,2,3,4,3,  2,1,0}, // String Index
            // Cmajor                                                                           Fmajor              Fm                
            new int[] { 0,1,2,0,1,2,0,1,  1,1,2,1,1,2,1,1,  2,1,2,2,1,2,1,2,  3,1,2,3,1,2,3,1,  0,1,2,1,  0,1,2,1,  0,0,2,0,  0,0,2,0,            
            //Cmajor            Cmajor Sweep
            2,3,2,3,  2,3,2,3,  2,5,2,3,2,4,5,4,2,3,2}, // Note Index            
            "Songs/Mine/Mine Medium (147bpm)" // Mp3 Path
            )
        },
        {
            "Cannon",
                new SongTemplate(
                "Cannon", // Song Name
                9, // Number of notes
                4, // Time sig
                4, // Time sig
                new int[] {2,2,4,4, 4,4,4,4,  4}, // dur
                new int[] {0,0,0,0, 2,2,1,1,  0}, // string
                new int[] {4,4,1,2, 1,3,1,2,  1 }, // notes
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
                new int[] {4,4,4,4,4,4,4,4,4,4,4,4,4,4}, // 14 durations
                new int[] {0,0,0,0,0,0,0,0,0,0,0,0,0,0},   // 14 strings
                new int[] {0,1,2,3,4,5,4,3,2,1,0,1,2,3},    // 14 notes
                "Songs/Mine/Mine Medium (100bpm)"
                )
        }
    };

    // Template for copy&paste
    //new SongTemplate(
    //            "", // Song Name
    //            12, // Number of notes
    //            4, // Time sig
    //            4, // Time sig
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