using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtDoomsGate
{
    // map.MapNote("",,8);
    // Note reference - https://github.com/robsoncouto/arduino-songs/blob/master/doom/doom.ino
    private SongMapping map;

    public void Map()
    {
        map = new SongMapping();
    }
    
    private void MapNotes()
    {
        // Name...Octave...Duration 
        // 4 is a quter note 8 is an eighteenth 16 is a sixteenth ext...
        // Negitive numbers stand for dotted notes, so -4 means a
        // quarter plus an eighteenth
        map.MapNote("e", 2, 8);
        map.MapNote("e", 2, 8);
        map.MapNote("e", 3, 8);
        map.MapNote("e", 2, 8);
        map.MapNote("e", 2, 8);
        map.MapNote("d", 3, 8);
        map.MapNote("e", 2, 8);
        map.MapNote("e", 2, 8);
        map.MapNote("c", 3, 8);
        map.MapNote("e", 2, 8);
        map.MapNote("e", 2, 8);
        map.MapNote("a#", 2, 8);
        map.MapNote("e", 2, 8);
        map.MapNote("e", 2, 8);
        map.MapNote("b", 2, 8);
        map.MapNote("c", 3, 8);
        map.MapNote("e", 2, 8);
        map.MapNote("e", 2, 8);
        map.MapNote("e", 3, 8);
        map.MapNote("e", 2, 8);
        map.MapNote("e", 2, 8);
        map.MapNote("d", 3, 8);
        map.MapNote("e", 2, 8);
        map.MapNote("e", 2, 8);
        map.MapNote("c", 3, 8);
        map.MapNote("e", 2, 8);
        map.MapNote("e", 2, 8);
        map.MapNote("a#", 2, -2);
        // 28 Notes but thers only 27 (im missing one ahhhhh)^^
    }
    



}