using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class containing a vector3 and id
public class TrailPoint
{
    public int id;
    public Vector3 position;

    public TrailPoint(int id_input, Vector3 position_input)
    {
        id = id_input;
        position = position_input;
    }
}
