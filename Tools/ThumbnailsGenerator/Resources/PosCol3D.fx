float4x4 worldViewProjection : WorldViewProjection;
float4x4 worldMatrix : WORLD;

float3 gLightDirection : LightDirection = { 0.577f, -0.577f, 0.577f };
float3 gCameraPos : CAMERA;

struct VS_INPUT
{
    float3 Position : POSITION;
    float3 normal : NORMAL;
};

struct VS_OUTPUT
{
    float4 Position : SV_POSITION;
    float3 normal : NORMAL;
    float3 ViewDirection : VERTEX_TO_CAMERA;
};

VS_OUTPUT VS(VS_INPUT input)
{
    VS_OUTPUT output = (VS_OUTPUT)0;

    // Transform position for projection
    output.Position = mul(float4(input.Position, 1.f), worldViewProjection);

    // Compute world-space position for ViewDirection calculation
    float3 worldPosition = mul(float4(input.Position, 1.f), worldMatrix).xyz;
    output.ViewDirection = normalize(gCameraPos - worldPosition);

    // Transform normal for world space and ensure it's normalized
    output.normal = normalize(mul(input.normal, (float3x3)worldMatrix));

    return output;
}

float4 PS_Point(VS_OUTPUT input) : SV_TARGET
{
    // Hardcoded grayscale values
    float4 ambientGray = float4(0.1f, 0.1f, 0.1f, 1.0f);
    float4 diffuseGray = float4(0.5f, 0.5f, 0.5f, 1.0f);
    float4 specularGray = float4(0.2f, 0.2f, 0.2f, 1.0f);

    // Ensure normalized normal and light direction
    float3 normal = normalize(input.normal);
    float3 lightDir = normalize(-gLightDirection);

    // Simple diffuse calculation in grayscale
    float cosAngle = max(dot(normal, lightDir), 0.0f);
    float4 diffuse = diffuseGray * cosAngle;

    // Simple specular calculation in grayscale
    float3 reflection = reflect(lightDir, normal);
    float cosAlpha = max(dot(reflection, input.ViewDirection), 0.0f);
    float4 specular = specularGray * pow(cosAlpha, 10.0f);

    // Final output color
    return ambientGray + diffuse + specular;
}

BlendState gBlendState
{
    BlendEnable[0] = false; 
    SrcBlend = one; 
    DestBlend = zero; 
    BlendOp = add; 
    SrcBlendAlpha = one; 
    DestBlendAlpha = zero; 
    BlendOpAlpha = add; 
    RenderTargetWriteMask[0] = 0x0F; 
};

RasterizerState gRasterizerState
{
    CullMode = none;
    FrontCounterClockwise = false;
};

DepthStencilState gDepthStencilState
{
    DepthEnable = true;
    DepthWriteMask = ALL;
    DepthFunc = less;
    StencilEnable = false;
};

technique11 DefaultTechnique
{
    pass P0
    {
        SetRasterizerState(gRasterizerState);
        SetDepthStencilState(gDepthStencilState, 0);
        SetBlendState(gBlendState, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
        SetVertexShader(CompileShader(vs_5_0, VS()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, PS_Point()));
    }
}
