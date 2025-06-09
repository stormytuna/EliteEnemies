sampler uImage0 : register(s0);

float2 screenSize;
float3 outlineColor;

float4 Outline(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 pixelSize = 1 / screenSize;
    float4 color = tex2D(uImage0, coords);
    if (any(color)) {
        return color;
    }

    float2 left = coords + float2(-2 * pixelSize.x, 0);
    if (any(tex2D(uImage0, left))) {
        return float4(outlineColor, 1);
    }

    float2 right = coords + float2(2 * pixelSize.x, 0);
    if (any(tex2D(uImage0, right))) {
        return float4(outlineColor, 1);
    }

    float2 top = coords + float2(0, -2 * pixelSize.y);
    if (any(tex2D(uImage0, top))) {
        return float4(outlineColor, 1);
    }

    float2 bottom = coords + float2(0, 2 * pixelSize.y);
    if (any(tex2D(uImage0, bottom))) {
        return float4(outlineColor, 1);
    }

    return float4(0, 0, 0, 0);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 Outline();
    }
}