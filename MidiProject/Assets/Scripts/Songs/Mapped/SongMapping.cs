using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongMapping
{
    // Individual notes which the user plays
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

    private void AddNoteToMap(MappedNote note)
    {
        tempSongMaps.Add(note);
    }

    
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

    public void RemoveNoteFromMapAndAddToNotes()
    {
        MappedNote nNote = songMap.Pop();
        notesPlayed.Add(nNote);
    }

    public void IncremeantScore(bool wasHit)
    {
        if (wasHit)
        {
            score += 1;
        }
    }

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

    // Creates and returns a new mapped note
    public void MapNote(string _noteName, int _noteOctave, float _duration)
    {
        MappedNote mNote = new MappedNote();
        mNote.noteName = _noteName;
        mNote.noteOctave = _noteOctave;
        mNote.duration = _duration;
        AddNoteToMap(mNote);
    }

    public bool IsMapFinalize()
    {
        if(isMapFinalize)
        {
            return true;
        }
        return false;
    }

}
