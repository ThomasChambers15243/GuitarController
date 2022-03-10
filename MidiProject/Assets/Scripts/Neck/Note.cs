using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Note : MonoBehaviour
{
    private string noteName;
    private float freq;

    private float minVoltage;
    private float maxVoltage;



    /// <summary>
    /// Constructor which sets the name and the frequency
    /// </summary>
    /// <param name="newName">Note name</param>
    /// <param name="octave">Note octave</param>
    public Note(string newName, int octave)
    {
        noteName = newName.ToUpper();
        setFreq(octave);
    }

    /// <summary>
    /// Sets the frequency of the note
    /// </summary>
    /// <param name="octave"></param>
    public void setFreq(int octave)
    {
        float recordFreq = FindNoteFreq(noteName, octave);

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
    public bool isNote(float inputVoltage)
    {
        if (inputVoltage >= minVoltage && inputVoltage <= maxVoltage)
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// Finds the frequency for the givin note
    /// </summary>
    /// <param name="note_Name">Notes name</param>
    /// <param name="octave">Notes octave</param>
    /// <returns></returns>
    private float FindNoteFreq(string note_Name, int octave)
    {
        string jsonString = File.ReadAllText("Assets/Scripts/Neck/noteData.json");
        JSONNode data = JSON.Parse(jsonString);

        foreach (JSONNode record in data)
        {
            // TODO TO SAVE PAIN...EDIT PYTHON SCRIPT TO CLEAN UP THE JSON 
            string recordName = record["Name"].ToString();            
            string recordOctave = record["Octave"].ToString();
            recordName = recordName.Remove(0, 1);
            recordName = recordName.Remove(recordName.Length - 1);
            recordName = recordName.Remove(recordName.Length - 1);
            recordOctave = recordOctave.Remove(0,1);
            recordOctave = recordOctave.Remove(recordOctave.Length - 1);
            

            if (recordName == note_Name && recordOctave == octave.ToString())
            {
                return (float)record["Freq"];
            }
        }
        return 0f;
    }

    public void SetVoltage(float desiredVoltage)
    {
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
}
