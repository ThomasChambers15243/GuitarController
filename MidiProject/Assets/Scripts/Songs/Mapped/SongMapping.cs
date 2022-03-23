using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongMapping
{
    /// <summary>
    /// Individual notes which the user plays
    /// </summary>
    public struct MappedNote
    {
        public string noteName { get; set; }
        public int noteOctave { get; set; }
        public float duration { get; set; }
    }

    // Check to see if the map is been finalized and completed.
    // Should only be set to true when FinalizeMap() is called
    private bool isMapFinalize = false;

    // Collection of mapped notes for the entire song
    public Stack<MappedNote> songMap = new Stack<MappedNote>();

    // Temp collections of notes for the map. Is then added to the stack
    // This means that when mapping songs, you don't have to add it backwards
    // but still get the benifits of the stack structure
    private List<MappedNote> tempSongMaps = new List<MappedNote>();

    // Collection of note that have been played, regardless of whether
    // they were hit or missed
    public  List<MappedNote> notesPlayed = new List<MappedNote>();

    // Total score
    public int score = 0;

    /// <summary>
    /// Appends the given note to the tempNotes List
    /// </summary>
    /// <param name="note"></param>
    private void AddNoteToMap(MappedNote note)
    {
        tempSongMaps.Add(note);
    }
    
    /// <summary>
    /// Adds all note to the map stack and declares the 
    /// map as complete
    /// </summary>
    public void FinalizeMap()
    {
        // Reverse the list so when its pushed
        // to the stack. the top of the stack is
        // the first note of the song
        tempSongMaps.Reverse();
        foreach(MappedNote n in tempSongMaps)
        {
            songMap.Push(n);
        }
        isMapFinalize = true;
    }

    /// <summary>
    /// Pops a note from the map and records 
    /// it as a played note
    /// </summary>
    private void RemoveNoteFromMapAndAddToNotes()
    {
        MappedNote nNote = songMap.Pop();
        notesPlayed.Add(nNote);
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

    /// <summary>
    /// Formates text for all notes that have been played
    /// </summary>
    /// <returns>String text</returns>
    public string NotesPlayed()
    {
        string text = "Notes played are: \n";
        foreach(MappedNote n in notesPlayed)
        {
            text += n.noteName;
            text += "\n";
        }
        return text;
    }

    /// <summary>
    /// Adds a note to the song map, from first note to last note
    /// </summary>
    /// <param name="_noteName">String name of the note</param>
    /// <param name="_noteOctave">Int octave of the note</param>
    /// <param name="_duration">Duration of the note as in musical notation, not seconds</param>
    public void MapNote(string _noteName, int _noteOctave, float _duration)
    {
        // Create note and add attributes 
        MappedNote mNote = new MappedNote();
        mNote.noteName = _noteName;
        mNote.noteOctave = _noteOctave;
        mNote.duration = _duration;        

        // Add note to map
        AddNoteToMap(mNote);
    }

    /// <summary>
    /// Check to see if the map is ready and complete
    /// </summary>
    /// <returns>Bool value, true if ready, else false</returns>
    public bool IsMapFinalize()
    {
        if(isMapFinalize)
        {
            return true;
        }
        return false;
    }

}
