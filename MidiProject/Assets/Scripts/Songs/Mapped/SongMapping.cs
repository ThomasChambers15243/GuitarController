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
        public float duration { get; set; }
        public int sIndex { get; set; }
        public int nIndex { get; set; }
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



    /// <summary>
    /// Adds a note to the song map, from first note to last note
    /// </summary>
    /// <param name="_noteName">String name of the note</param>
    /// <param name="_noteOctave">Int octave of the note</param>
    /// <param name="_duration">Duration of the note as in musical notation, not seconds</param>
    public void MapNote(float _duration, int _sIndex, int _nIndex)
    {
        // Create note and add attributes 
        MappedNote mNote = new MappedNote();
        mNote.duration = _duration;
        mNote.sIndex = _sIndex;
        mNote.nIndex = _nIndex;
        // Add note to map
        AddNoteToMap(mNote);
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
        if(!IsMapFinalized())
        {
            tempSongMaps.Reverse();
            foreach (MappedNote n in tempSongMaps)
            {
                songMap.Push(n);
            }
            isMapFinalize = true;
        }
    }

    /// <summary>
    /// Check to see if the map is ready and complete
    /// </summary>
    /// <returns>Bool value, true if ready, else false</returns>
    public bool IsMapFinalized()
    {
        if(isMapFinalize)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Pops a note from the map and records 
    /// it as a played note
    /// </summary>
    public MappedNote RemoveNoteFromMap()
    {
        MappedNote nNote = songMap.Pop();
        return nNote;
    }
    
    /// <summary>
    /// Appends the given note to the tempNotes List
    /// </summary>
    /// <param name="note"></param>
    private void AddNoteToMap(MappedNote note)
    {
        tempSongMaps.Add(note);
    }
   
}
