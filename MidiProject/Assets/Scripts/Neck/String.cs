using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class String
{    
    public string tunning;
    // Starting octave of the first note on the string
    public int octave;
    public float maxVoltage = 5f;
    public Note[] notes = new Note[6];



    /// <summary>
    /// Fills up notes array with correct notes according to objects tunnings.
    /// </summary>
    public void SetNotes()
    {
        int tempOctave = octave;
        int noteCount = Array.IndexOf(Tunnings.allOctaveNotes, tunning);

        for (int i = 0; i < 6; i++)
        {
            Note note = new Note(Tunnings.allOctaveNotes[noteCount], tempOctave);
            notes[i] = note;

            // Loops back around to the start of the array
            // if it reaches then end increases octave
            if (noteCount%11 == 0 && noteCount != 0)
            {
                noteCount = 0;
                tempOctave += 1;
            }
            else
            {
                noteCount += 1;
            }
        }
    }

    /// <summary>
    /// Sets range of freqs relitive to their position on the neck
    /// </summary>
    public void SetVoltageRangeOfNotes()
    {
        for(int i = 0; i < 6; i++)
        {            
            //TODO change back to voltage from fret as this is for board testing
            //notes[i].SetVoltage(Tunnings.voltageFromFret1[i]);
            notes[i].SetVoltage(Tunnings.voltageFromBreadBoard[i]);
        }
    }

    /// <summary>
    /// Finds the note corresponding to the givin voltage
    /// </summary>
    /// <param name="voltage">The voltage from the guitar reading</param>
    /// <returns>The note if found, else null</returns>
    public Note GetNote(float voltage)
    {
        int index = GetNoteIndex(voltage); 
        return notes[index];
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
            if (notes[i].IsNote(voltage))
            {
                return i;
            }
        }
        return -1;
    }
}
