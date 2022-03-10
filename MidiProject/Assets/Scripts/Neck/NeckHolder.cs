using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class NeckHolder : MonoBehaviour
{

    
    private String[] strings = new String[6];

    /// <summary>
    /// Constructor which fills the strings array with string object
    /// </summary>
    /// <param name="guitarTunning">String[] of tunnings for each string, from thin to thick string</param>
    public NeckHolder(string[] guitarTunning)
    {
        for (int i = 0; i < 6; i++)
        {
            // Settings strings to standard tunnings
            String guitarString = new String();
            guitarString.tunning = guitarTunning[i];
            guitarString.octave = 4;
            guitarString.SetNotes();
            strings[i] = guitarString;            
        }
    }


    /// <returns>Array of String objects</returns>
    public String[] GetStrings()
    {
        return strings;
    }

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
