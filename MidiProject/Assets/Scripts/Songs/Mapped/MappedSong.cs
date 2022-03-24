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

    // Name of Map
    public string name{get;}

    // Collection of note that have been played, regardless of whether
    // they were hit or missed
    public List<SongMapping.MappedNote> notesPlayed = new List<SongMapping.MappedNote>();

    // Current note thats been poped of map stack
    public SongMapping.MappedNote currentNote = new SongMapping.MappedNote();

    private int score;

    public MappedSong(string s_name)
    {
        name = s_name;
        song = SongToMapData.GetSongToMapData(name);
        MapInnit();
    }

    public void PlayNote(int tempo)
    {
        currentNote = map.RemoveNoteFromMap();

        // Adds the last popped note to the list tracking played notes
        SongMapping.MappedNote lastPlayedNote = new SongMapping.MappedNote();
        notesPlayed.Add(lastPlayedNote);
    }

    /// <summary>
    /// Incremeants the score if the note was hit
    /// </summary>
    /// <param name="wasHit">True if the player hit the note, false if they missed</param>
    public void IncremeantScore(bool wasHit)
    {
        if (wasHit)
        {
            score += 1;
        }
    }



    private void MapInnit()
    {
        map = new SongMapping();
        MapNotes();
        map.FinalizeMap();
    }


    /// <summary>
    /// Add all the notes the player will have to play
    /// </summary>
    private void MapNotes()
    {
        if (!map.IsMapFinalize())
        {
            for(int i = 0; i < song.noteCount; i++)
            {
                map.MapNote(song.nNames[i], song.nOctave[i], song.nDur[i]);
            }
        }
    }
}
