using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISongMusicalDetails
{
    string name { get; set; }
    float tempo { get; set; }
    int[] melody { get; set; }
    int noteAmount { get; set; }
    int wholeNote { get; set; }
    int divider { get; set; }

    void PlayMelody();
    
}
