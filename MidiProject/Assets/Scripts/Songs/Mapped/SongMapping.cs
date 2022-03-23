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
    // Collection of mapped notes for the entire song
    public Stack<MappedNote> songMap = new Stack<MappedNote>();

    // Collection of note that have been played, regardless of whether
    // they were hit or missed
    public  List<MappedNote> notesPlayed = new List<MappedNote>();

    // Total score
    public int score = 0;

    public void AddNoteToMap(MappedNote note)
    {
        songMap.Push(note);
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

}
