using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

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
    // Extra note at the start so timmings are shifted
    // Extra note at the end too
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
