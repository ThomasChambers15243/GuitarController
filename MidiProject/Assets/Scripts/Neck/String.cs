using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class String : MonoBehaviour
{
    public string tunning;
    public int octave;

    public float maxVoltage = 5f;
    public Note[] notes = new Note[6];



    /// <summary>
    /// Fills up notes array with correct notes according to objects tunnings.
    /// </summary>
    public void SetNotes()
    {
        int noteCount = Array.IndexOf(Tunnings.allOctaveNotes, tunning);

        for (int i = 0; i < 6; i++)
        {
            Note note = new Note(Tunnings.allOctaveNotes[noteCount], octave);
            notes[i] = note;

            if (noteCount%11 == 0)
            {
                noteCount = 0;
            }
            else
            {
                noteCount += 1;
            }
        }
    }

    // Sets range of freqs relitive to their position on the neck
    public void SetFreqRangeOfNotes()
    {

    }

    /// <summary>
    /// Finds the note corresponding to the givin voltage
    /// </summary>
    /// <param name="voltage">The voltage from the guitar reading</param>
    /// <returns>The note if found, else null</returns>
    public Note GetNote(float voltage)
    {
        int index = GetNoteIndex(voltage);

        if (index != -1)
        {
            return notes[index];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Finds the index in notes for the corresponding note
    /// to the givin voltage
    /// </summary>
    /// <param name="voltage">The voltage from the guitar reading</param>
    /// <returns>The index of the note in notes if found, else -1</returns>
    public int GetNoteIndex(float voltage)
    {
        for (int i = 0; i < notes.Length; i++)
        {
            if (notes[i].isNote(voltage))
            {
                return i;
            }
        }
        return -1;
    }



}
