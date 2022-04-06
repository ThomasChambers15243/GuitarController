using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class NeckHolder
{
    // Stores all strings
    private String[] strings = new String[6];

    /// <summary>
    /// Constructor which fills the strings array with string objects
    /// </summary>
    /// <param name="guitarTunning">String[] of tunnings for each string, from thin to thick string</param>
    public NeckHolder(string[] guitarTunning)
    {
        for (int i = 0; i < 6; i++)
        {
            // Settings strings to standard tunnings
            String guitarString = new String();
            guitarString.tunning = guitarTunning[i];

            // Set octave of strings in accordance to standard tunned octaves
            if (i == 0)
            {
                guitarString.octave = 4;
            }
            else if (i < 4)
            {
                guitarString.octave = 3;
            }
            else
            {
                guitarString.octave = 2;
            }            
            guitarString.SetNotes();
            guitarString.SetVoltageRangeOfNotes();
            strings[i] = guitarString;            
        }
    }

    /// <summary>Getter for string array</summary>
    /// <returns>Array of String objects</returns>
    public String[] GetStrings()
    {
        return strings;
    }

    /// <summary>Gets the string index in the string array for the string number</summary>
    /// <param name="stringNumber"></param>
    /// <returns>String object at the specific number</returns>
    public String GetStringAtNumber(int stringNumber)
    {
        if(stringNumber >= 0 && stringNumber <= 6)
        {
            return strings[stringNumber];
        }
        else
        {
            return null;
        }
    }

}
