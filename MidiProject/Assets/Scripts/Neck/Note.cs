using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Note : MonoBehaviour
{
    private string noteName;
    private float freq;
    struct voltageRange
    {
        float minVoltage;
        float maxVoltage;
    }

    public Note(string newName, int octave)
    {
        noteName = newName.ToUpper();
        setFreq(octave);
    }

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

    // Checks to see if the input voltage is within in the votlage range
    // if so it returns true else false
    public bool isNote(int inputVoltage) { return false; }

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
    public string GetName()
    {
        return noteName;
    }
    public float GetFreq()
    {
        return freq;
    }
}
