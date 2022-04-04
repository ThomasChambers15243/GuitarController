using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

/// <summary>
/// Template for all song map data to be written as
/// </summary>
public class SongTemplate
{
    public string songName;
    public int noteCount;
    public int timeSigTop;
    public int timeSigBot;
    public string[] nNames;
    public int[] nOctave;
    public int[] nDur;
    public int[] sIndex;
    public int[] nIndex;
    
    public SongTemplate(string _songName, int _noteCount, int _timeSigTop, int _timeSigBot,
        string[] _nNames, int[] _nOctave, int[] _nDur, int[] _sIndex, int[] _nIndex)
    {        
        songName = _songName;
        noteCount = _noteCount;
        timeSigTop = _timeSigTop;
        timeSigBot = _timeSigBot;
        nNames = _nNames;
        nOctave = _nOctave;
        nDur = _nDur;
        sIndex = _sIndex;
        nIndex = _nIndex;
    }
}
