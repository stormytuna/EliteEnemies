sampler uImage0 : register(s0);
float3 rainbow;

float4 Rainbow(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 sample = tex2D(uImage0, coords);

    float value = 0.3 * sample.r + 0.59 * sample.g + 0.11 * sample.b;
    float3 tint = float3(1, 0.2, 0.2);
    float tintMix = 0.8;
    float outR = tintMix * value * rainbow.r + (1 - tintMix) * sample.r;
    float outG = tintMix * value * rainbow.g + (1 - tintMix) * sample.g;
    float outB = tintMix * value * rainbow.b + (1 - tintMix) * sample.b;

    return float4(outR, outG, outB, sample.a) * sampleColor;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 Rainbow();
    }
}