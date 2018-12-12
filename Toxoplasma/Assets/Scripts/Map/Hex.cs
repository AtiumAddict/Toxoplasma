using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// It defines the gid position, world space position, size, neighbours, etc. of a Hexagon. 
/// </summary>
public class Hex
{
    public Hex(int q, int r)
    {
        this.Q = q;
        this.R = r;
        this.S = -(q + r);
    }

    // Q + R + S = 0
    // S = -(Q + R)
    public readonly int Q; // Column
    public readonly int R; // Row
    public readonly int S;

    readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;

    /// <summary>
    /// Returns the world-space positon of the hex
    /// </summary>
    /// <returns></returns>
    public Vector3 Position ()
    {
        float radius = 1f;
        float height = radius * 2;
        float width = WIDTH_MULTIPLIER * height;

        float vert = height * 0.75f;
        float horiz = width;
        
        return new Vector3(
            horiz * (this.Q + this.R/2f),
            0,
            vert * this.R
            );
    }
}
