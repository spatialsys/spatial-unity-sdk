Shader "Hidden/SpatialSys/Utilities/TexturePacker"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }

        _RedTex ("Red Texture", 2D) = "white" { }
        _GreenTex ("Green Texture", 2D) = "white" { }
        _BlueTex ("Blue Texture", 2D) = "white" { }
        _AlphaTex ("Alpha Texture", 2D) = "white" { }

        _RedChannel ("Red Channel", Int) = 0
        _GreenChannel ("Green Channel", Int) = 0
        _BlueChannel ("Blue Channel", Int) = 0
        _AlphaChannel ("Alpha Channel", Int) = 0

        _RedDefault ("Red Default", Float) = 1
        _GreenDefault ("Red Default", Float) = 1
        _BlueDefault ("Red Default", Float) = 1
        _AlphaDefault ("Red Default", Float) = 1

        _RedInvert ("Red Invert", Int) = 0
        _GreenInvert ("Green Invert", Int) = 0
        _BlueInvert ("Blue Invert", Int) = 0
        _AlphaInvert ("Alpha Invert", Int) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _MainTex_ST;
            sampler2D _MainTex;

            sampler2D _RedTex;
            sampler2D _GreenTex;
            sampler2D _BlueTex;
            sampler2D _AlphaTex;

            int _RedChannel;
            int _GreenChannel;
            int _BlueChannel;
            int _AlphaChannel;

            float _RedDefault;
            float _GreenDefault;
            float _BlueDefault;
            float _AlphaDefault;

            int _RedInvert;
            int _GreenInvert;
            int _BlueInvert;
            int _AlphaInvert;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float red, green, blue, alpha;
                if (_RedChannel == -1)
                {
                    red = _RedDefault;
                }
                else
                {
                    red = tex2D(_RedTex, i.uv)[_RedChannel];
                    if (_RedInvert == 1)
                    {
                        // // Gamma correction
                        // half3 redLinear = LinearToGammaSpace(red);
                        // redLinear = 1 - redLinear;
                        // red = GammaToLinearSpace(redLinear);
                        red = 1 - red;
                    }
                }
                if (_GreenChannel == -1)
                {
                    green = _GreenDefault;
                }
                else
                {
                    green = tex2D(_GreenTex, i.uv)[_GreenChannel];
                    if (_GreenInvert == 1)
                    {
                        // // Gamma correction
                        // half3 greenLinear = LinearToGammaSpace(green);
                        // greenLinear = 1 - greenLinear;
                        // green = GammaToLinearSpace(greenLinear);
                        green = 1 - green;
                    }
                }
                if (_BlueChannel == -1)
                {
                    blue = _BlueDefault;
                }
                else
                {
                    blue = tex2D(_BlueTex, i.uv)[_BlueChannel];
                    if (_BlueInvert == 1)
                    {
                        // // Gamma correction
                        // half3 blueLinear = LinearToGammaSpace(blue);
                        // blueLinear = 1 - blueLinear;
                        // blue = GammaToLinearSpace(blueLinear);
                        blue = 1 - blue;
                    }
                }
                if (_AlphaChannel == -1)
                {
                    alpha = _AlphaDefault;
                }
                else
                {
                    alpha = tex2D(_AlphaTex, i.uv)[_AlphaChannel];
                    if (_AlphaInvert == 1)
                    {
                        // // Gamma correction
                        // half3 alphaLinear = LinearToGammaSpace(_AlphaDefault);
                        // alphaLinear = 1 - alphaLinear;
                        // alpha = GammaToLinearSpace(alphaLinear);
                        alpha = 1 - alpha;
                    }
                }
                return float4(red, green, blue, alpha);
            }
            ENDCG
        }
    }
}
