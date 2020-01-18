#include "UnityCG.cginc"
#include "UnityGBuffer.cginc"
#include "UnityStandardUtils.cginc"
#include "GeometryHelper.cginc"

struct HairData
{
	float accelX[50];
	float accelY[50];
	float accelZ[50];
	float velX[50];
	float velY[50];
	float velZ[50];
	float posX[50];
	float posY[50];
	float posZ[50];
};

struct StrandData
{
	float3 anchor;
	int length;
};

struct VertexData
{
	float4 currentPos : POSITION0;
	float4 nextPos : POSITION1;
	float3 nextNorm : NORMAL0;
};

struct GeometryData
{
	float4 pos : POSITION;
	float3 norm : NORMAL;
};

float4 _Color = float4(1, 1, 1, 1);

StructuredBuffer<HairData> HairBuf;
StructuredBuffer<StrandData> DataBuf;
float HairThickness;
int HairDensity;

VertexData Vertex(uint vid : SV_VertexID)
{
	int si = vid / 50, bi = vid % 50;
	HairData strandData = HairBuf[si];

	VertexData output;
	output.currentPos = float4(strandData.posX[bi], strandData.posY[bi], strandData.posZ[bi], 1.0f);

	int hairLength = DataBuf[si].length;
	if (bi < hairLength - 1)
	{
		output.nextPos = float4(strandData.posX[bi + 1], strandData.posY[bi + 1], strandData.posZ[bi + 1], 1.0f);
		if (bi < hairLength - 2)
		{
			float4 tempVertex = float4(strandData.posX[bi + 2], strandData.posY[bi + 2], strandData.posZ[bi + 2], 1.0f);
			output.nextNorm = float3(normalize(tempVertex - output.nextPos).xyz);
		}
	}
	return output;
}

[maxvertexcount(64)]
void Geometry(point VertexData input[1], uint pid : SV_PrimitiveID, inout TriangleStream<GeometryData> outStream)
{
	int si = pid / 50, bi = pid % 50;
	int hairLength = DataBuf[si].length;

	float4 currentPos = input[0].currentPos;
	float4 nextPos = input[0].nextPos;
	float3 currentNorm = normalize(float3(nextPos.xyz) - float3(currentPos.xyz));
	float3 nextNorm = (bi < hairLength - 2) ? input[0].nextNorm : currentNorm;
	float3 currentPerp = GetPerpNorm(currentNorm);
	float3 nextPerp = GetPerpNorm(nextNorm);

	GeometryData output;
	if (bi < hairLength - 1)
	{
		for (int i = 0; i < HairDensity + 1; i++)
		{
			float3 tempRingNorm = AngleAxisRotation(currentPerp, (360 / HairDensity) * (i % HairDensity), currentNorm);
			float4 newPos = currentPos + float4(tempRingNorm.xyz, 0) * HairThickness;
			output.pos = mul(UNITY_MATRIX_VP, float4(newPos.xyz, 1.0f));
			output.norm = UnityObjectToWorldNormal(tempRingNorm);
			outStream.Append(output);

			tempRingNorm = AngleAxisRotation(nextPerp, (360 / HairDensity) * (i % HairDensity), nextNorm);
			newPos = nextPos + float4(tempRingNorm.xyz, 0) * HairThickness;
			output.pos = mul(UNITY_MATRIX_VP, float4(newPos.xyz, 1.0f));
			output.norm = UnityObjectToWorldNormal(tempRingNorm);
			outStream.Append(output);
		}
	}
	outStream.RestartStrip();
}

float4 Fragment(GeometryData input) : COLOR
{
	float4 color = float4(input.norm.xyz * 0.5f + 0.5f, 1.0f);
	return color;
}