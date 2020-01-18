//File Name: Hair.cs
//Description: This file handles the hair system. This will eventually be broken down
//when hair creation becomes more detailed. For now this file just does hair.
//Programmer: Kyle Jensen
//Revision Date: February 3, 2018

using System.Runtime.InteropServices;
using UnityEngine;

public class Hair : MonoBehaviour
{
    private const int kMaxHairLength = 50;
    private const int kMaxHairStrands = 2048;

    private int kernelID;
    private ComputeBuffer inputBuffer;
    private ComputeBuffer outputBuffer;
    private ComputeBuffer varyingBuffer;
    private Vector3 originPosition;
    private HairStrand[] hairStrands;
    private StrandData[] strandData;

    public StrandData[] StrandData { get { return strandData; } }

    [Header("Strand Details")]
    public float strandSpacing;
    public int numRows;
    public int length;
    public bool randomizeHairLengths;
    public int bodyCount;

    [Header("Hair Physics Details")]
    public ComputeShader computeShader;
    public float mass = 100.0f;
    public float springLength = 1.0f;
    public float springStiffness = 100.0f;
    public float dampening = 0.1f;

    [Header("Hair Shader Details")]
    public int density;
    public float thickness;
    public Shader geometryShader;
    private Material geometryMaterial;

    [Header("Debug")]
    public bool resetShaderInfo;
    public bool waveStrands;
    public float waveMultiplier;
    public bool drawGizmos;

    /// <summary>
    /// The load method initializes all the hair strands in a grid. It creates the hair strands at
    /// a length and adds them to the array. It then initilizes the kernel and the compute buffers for the compute shader.
    /// </summary>
    public void Load()
    {
        this.hairStrands = new HairStrand[numRows * numRows];
        this.strandData = new StrandData[numRows * numRows];
        this.originPosition = transform.position;
        this.bodyCount = 0;

        for (int r = 0; r < numRows; r++)
        {
            for (int c = 0; c < numRows; c++)
            {
                int index = r * numRows + c;
                int strandLength = (randomizeHairLengths) ? Random.Range(1, length) : length;
                Vector3 newPosition = originPosition + new Vector3(c * -strandSpacing, 0, r * -strandSpacing);
                hairStrands[index] = CreateHairStrand(strandLength, newPosition);
                strandData[index].anchor = newPosition;
                strandData[index].length = strandLength;
                bodyCount += strandLength;
            }
        }

        this.geometryMaterial = new Material(geometryShader);

        this.kernelID = computeShader.FindKernel("CSMain");
        this.inputBuffer = new ComputeBuffer(hairStrands.Length, Marshal.SizeOf(typeof(HairStrand)));
        this.outputBuffer = new ComputeBuffer(hairStrands.Length, Marshal.SizeOf(typeof(HairStrand)));
        this.varyingBuffer = new ComputeBuffer(strandData.Length, Marshal.SizeOf(typeof(StrandData)));
        this.inputBuffer.SetData(hairStrands);
        this.outputBuffer.SetData(hairStrands);
        this.varyingBuffer.SetData(strandData);
        this.SetHairShaderInfo();
    }

    /// <summary>
    /// This is the fixed update loop that runs the physics portion of the hair simulation.
    /// The input buffer is set with the current values of the hair strands array, the buffers
    /// are set on the compute shader, and then the shader is dispatched. The data is read back
    /// into the hair strands array from the output buffer.
    /// </summary>
    void FixedUpdate()
    {
        if(waveStrands) //For debugging purposes only, moves strands back and forth to see movement
        {
            for (int i = 0; i < strandData.Length; i++)
            {
                strandData[i].anchor.x += Mathf.Sin(Time.time) * waveMultiplier;
            }
        }

        varyingBuffer.SetData(strandData);
        computeShader.SetBuffer(kernelID, "ReadBuf", inputBuffer);
        computeShader.SetBuffer(kernelID, "WriteBuf", outputBuffer);
        computeShader.SetBuffer(kernelID, "Varying", varyingBuffer);
        computeShader.Dispatch(kernelID, numRows * numRows, 1, 1);

        //Buffer swap
        ComputeBuffer tempBuffer = inputBuffer;
        inputBuffer = outputBuffer;
        outputBuffer = tempBuffer;
    }

    /// <summary>
    /// This event is triggered once per frame and draws the hair procedurally based on the 
    /// hair points. The geometry shader is loaded and then drawn.
    /// </summary>
    private void OnRenderObject()
    {
        geometryMaterial.SetPass(0);
        geometryMaterial.SetBuffer("HairBuf", outputBuffer);
        geometryMaterial.SetBuffer("DataBuf", varyingBuffer);
        geometryMaterial.SetFloat("HairThickness", thickness);
        geometryMaterial.SetInt("HairDensity", density);
        Graphics.DrawProcedural(MeshTopology.Points, outputBuffer.count * kMaxHairLength);
    }

    /// <summary>
    /// This method creates a hair strand with a given length and anchor position.
    /// </summary>
    /// <param name="length">The length of the hair (number of hair bodies)</param>
    /// <param name="anchorPosition">The anchor position that the hair will be attached to.</param>
    /// <returns>A new hair strand</returns>
    private unsafe HairStrand CreateHairStrand(int length, Vector3 anchorPosition)
    {
        HairStrand strand = new HairStrand();
        for (int i = 0; i < length && i < kMaxHairLength; i++)
        {
            Vector3 newPos = anchorPosition + (-Vector3.up * (springLength * i));
            strand.posX[i] = newPos.x;
            strand.posY[i] = newPos.y;
            strand.posZ[i] = newPos.z;
        }
        return strand;
    }

    /// <summary>
    /// This method sets the shader info that the shader needs to execute.
    /// </summary>
    private void SetHairShaderInfo()
    {
        computeShader.SetFloat("TimeDelta", Time.fixedDeltaTime);
        computeShader.SetFloat("Mass", mass);
        computeShader.SetFloat("SpringLength", springLength);
        computeShader.SetFloat("Stiffness", springStiffness);
        computeShader.SetFloat("Dampening", dampening);
        computeShader.SetVector("Gravity", Physics.gravity);
    }
}