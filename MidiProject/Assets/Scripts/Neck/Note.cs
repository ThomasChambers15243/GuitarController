using SimpleJSON;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Note
{
    // Note data
    private string noteName;
    private float freq;
    private int noteOctave;

    // Best case voltage
    private float perfectVoltage;

    // Range of acceptable voltages
    public float minVoltage;
    public float maxVoltage;



    /// <summary>
    /// Constructor which sets the name and the frequency of the note
    /// </summary>
    /// <param name="newName">Note name</param>
    /// <param name="octave">Note octave</param>
    public Note(string newName, int octave)
    {
        noteName = newName.ToUpper();
        setFreq(octave);
        noteOctave = octave;
    }
    
    /// <summary>
    /// Sets the frequency of the note
    /// </summary>
    /// <param name="octave">Ocatave for the note</param>
    public void setFreq(int octave)
    {
        float recordFreq = FindNoteFreq(noteName, octave);

        // Set ntoe frequency if found
        if (recordFreq != 0f)
        {
            freq = recordFreq;
        }
        else
        {
            Debug.Log("Could not find frequency. " + "Name was: " + noteName + " and octave was " + octave);
        }
    }

    /// <summary>
    /// Checks to see if the input voltage is within range
    /// for the note
    /// </summary>
    /// <param name="inputVoltage">Voltage to be checked against the min-max voltage</param>
    /// <returns>true if note, false if note</returns>
    public bool IsNote(float inputVoltage)
    {
        if (inputVoltage >= minVoltage && inputVoltage <= maxVoltage)
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// Finds the frequency for the givin note at the given octave
    /// </summary>
    /// <param name="note_Name">Notes name</param>
    /// <param name="octave">Notes octave</param>
    /// <returns>Freqency of note, 0 if not found</returns>
    private float FindNoteFreq(string note_Name, int octave)
    {
        // Load json note data
        string jsonString = File.ReadAllText("Assets/Scripts/Neck/noteData.json");
        JSONNode data = JSON.Parse(jsonString);

        
        foreach (JSONNode record in data)
        {
            // Cleans each json record so it can be checks
            string recordName = record["Name"].ToString();            
            string recordOctave = record["Octave"].ToString();
            recordName = recordName.Remove(0, 1);
            recordName = recordName.Remove(recordName.Length - 1);
            recordName = recordName.Remove(recordName.Length - 1);
            recordOctave = recordOctave.Remove(0,1);
            recordOctave = recordOctave.Remove(recordOctave.Length - 1);
            
            // Compares record to note data to find if they are the same note
            // if true, returns the notes frequency
            if (recordName == note_Name && recordOctave == octave.ToString())
            {
                return (float)record["Freq"];
            }
        }
        return 0f;
    }

    /// <summary>
    /// Sets the perfect voltage and voltage range of the note
    /// </summary>
    /// <param name="desiredVoltage">Perfect voltage of the note</param>
    public void SetVoltage(float desiredVoltage)
    {
        perfectVoltage = desiredVoltage;
        // Calculates one percent of th voltage and
        // sets min and max range of the notes voltage
        float onePercent = (desiredVoltage / 100);
        minVoltage = desiredVoltage - onePercent;
        maxVoltage = desiredVoltage + onePercent;
    }
    
    /// <summary>
    /// Gets the name of the note
    /// </summary>
    /// <returns>returns the note name</returns>
    public string GetName()
    {
        return noteName;
    }

    /// <summary>
    /// Gets the note frequency
    /// </summary>
    /// <returns>Returns the frequency</returns>
    public float GetFreq()
    {
        return freq;
    }

    /// <summary>
    /// Gets the notes octave
    /// </summary>
    /// <returns>Notes octave</returns>
    public int GetOcatve()
    {
        return noteOctave;
    }

    /// <summary>
    /// Concatenates the notes name and octave and 
    /// returns it in the format nameoctave (e.g c5)
    /// </summary>
    /// <returns>Note name and octave concatenated</returns>
    public string GetNameWithOctave()
    {
        return (string) noteName + noteOctave;
    }

    /// <summary>
    /// Gets the notes perfect voltage
    /// </summary>
    /// <returns>Notes perfect voltage</returns>
    public float GetPerfectVoltage()
    {
        return perfectVoltage;
    }

}
