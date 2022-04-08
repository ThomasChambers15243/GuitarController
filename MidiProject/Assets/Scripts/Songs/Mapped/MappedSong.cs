using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MappedSong
{
    // Note reference - https://github.com/robsoncouto/arduino-songs/blob/master/doom/doom.ino

    // Map object inwhich all song data will be stored
    private SongMapping map;
    
    // Template inwhcih to load song data from 
    private SongTemplate song;

    // Number of notes in the map
    private int lengthOfMap;

    // Name of Map
    public string name{get;}

    // File path to mp3 of song
    public string sFilePath;

    // Time Signiture of map e.g 4/4
    public int timesSigTop;
    public int timesSigBot;

    // Collection of note that have been played, regardless of whether
    // they were hit or missed
    public List<SongMapping.MappedNote> notesPlayed = new List<SongMapping.MappedNote>();

    // Current note thats been poped of map stack
    public SongMapping.MappedNote currentNote = new SongMapping.MappedNote();

    /// <summary>
    /// Constructor which loads in song data and innits the mapo
    /// </summary>
    /// <param name="s_name">Map name</param>
    public MappedSong(string s_name)
    {
        name = s_name;
        song = SongToMapData.GetSongToMapData(name);
        lengthOfMap = song.noteCount;
        timesSigTop = song.timeSigTop;
        timesSigBot = song.timeSigBot;
        sFilePath = song.sFilePath;
        MapInnit();
    }

    /// <summary>
    /// Gets the next note from the map loads it as currentNote
    /// and also adds it to the lastPlayedNote list
    /// </summary>
    public void PlayNote()
    {
        currentNote = map.RemoveNoteFromMap();
        
        // Adds the last popped note to the list tracking played notes
        SongMapping.MappedNote lastPlayedNote = new SongMapping.MappedNote();
        notesPlayed.Add(lastPlayedNote);
    }

    /// <summary>
    /// Creats a new map, loads the notes 
    /// and locks the map as finalized
    /// </summary>
    private void MapInnit()
    {
        map = new SongMapping();
        MapNotes();
        map.FinalizeMap();
    }


    /// <summary>
    /// Adds all the notes the player will have to play
    /// </summary>
    private void MapNotes()
    {
        if (!map.IsMapFinalized())
        {
            for(int i = 0; i < song.noteCount; i++)
            {
                map.MapNote(song.nDur[i], song.sIndex[i], song.nIndex[i]);
            }
        }
    }

    /// <summary>
    /// Finds the length of the map
    /// </summary>
    /// <returns>The length of the mop</returns>
    public int GetLengthOfMap()
    {
        return lengthOfMap;
    }
}
