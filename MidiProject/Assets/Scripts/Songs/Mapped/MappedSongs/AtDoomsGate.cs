using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtDoomsGate
{
    private SongMapping map;

    public void Map()
    {
        map = new SongMapping();
    }
    
    private void MapNotes()
    {
        map.MapNote("a#", 2, -2);
        map.MapNote("",,);

    }
    



}