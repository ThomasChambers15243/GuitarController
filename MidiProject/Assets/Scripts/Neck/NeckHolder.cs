using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class NeckHolder : MonoBehaviour
{

    
    public String[] strings = new String[6];
    
    public NeckHolder()
    {
        for (int i = 0; i < 6; i++)
        {
            // Settings strings to standard tunnings
            String guitarString = new String();
            guitarString.tunning = Tunnings.standardTunning[i];
            guitarString.octave = 4;
            guitarString.SetNotes();
            strings[i] = guitarString;            
        }
    }





}
