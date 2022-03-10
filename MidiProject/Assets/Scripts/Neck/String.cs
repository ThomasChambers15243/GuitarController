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



    float GetInputVoltage()
    {
        return 1f;
    }

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

    // Loops through notes until notes.isNote() returns true then returns that note -- Could be recursive?
    void GetNote(float voltage) // return type Note
    {

    }

}
