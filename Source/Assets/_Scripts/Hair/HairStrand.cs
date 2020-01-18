using UnityEngine;

/// <summary>
/// This is the hair strand struct for a hair strand. We use an object of arrays approach because
/// C# does not handle fixed length arrays of arbitrary objects. We must use floats to interface
/// correctly with the compute shader.
/// </summary>
public unsafe struct HairStrand
{
    public fixed float accelX[50];
    public fixed float accelY[50];
    public fixed float accelZ[50];
    public fixed float velX[50];
    public fixed float velY[50];
    public fixed float velZ[50];
    public fixed float posX[50];
    public fixed float posY[50];
    public fixed float posZ[50];
}

public struct StrandData
{
    public Vector3 anchor;
    public int length;
}