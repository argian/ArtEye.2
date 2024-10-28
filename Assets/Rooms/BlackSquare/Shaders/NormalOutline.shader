Shader "Custom/NormalOutline"
{
    Properties
    {
        _LineColor("LineColor", Color) = (0, 0, 0, 0)
        _LineWidth("LineWidth", int) = 1
        _NormalsThreshold("NormalsTreshold", Range(0, 1)) = 1
        _DepthThreshold("DepthTreshold", Range(0, 1)) = 1
    }
            SubShader
        {
            // Draw after all opaque geometry
            //well not exacly
            Tags {
                "RenderType" = "Opaque"
                "Queue" = "Overlay-2001"
                "LightMode" = "Always"
                "IgnoreProjector" = "True"
                "DisableBatching" = "True"
                "ForceNoShadowCasting" = "True"
                "PreviewType" = "Plane"
                "VRCFallback" = "Hidden"
            }

            //Blend One Zero
            ZWrite Off
            ZTest Always
            Cull Off

            // Grab the screen behind the object into _BackgroundTexture
            GrabPass
            {
                "_GrabTex"
            }

            // Render the object with the texture generated above, and invert the colors
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
                #include "Assets/Shaders/UsefulCalculations.cginc"

                struct v2f
                {
                    float4 grabPos : TEXCOORD0;
                    float4 pos : SV_POSITION;
                };

                v2f vert(appdata_base v)
     {
    v2f o;

    o.pos = UnityObjectToClipPos(v.vertex);

    o.grabPos = ComputeGrabScreenPos(o.pos);
    return o;
    }

    sampler2D _GrabTex;

    int _LineWidth;
    float _NormalsThreshold;
    float _DepthThreshold;

    half4 frag(v2f i) : SV_Target
    {
        float2 uv = i.grabPos.xy / i.grabPos.w;
        half4 col = tex2D(_GrabTex, uv);
        //reformat
        col = col * 2 - 1;

        //fraction take:
        float2 rounduv = float2(round(uv.x * _ScreenParams.x) / _ScreenParams.x, round(uv.y * _ScreenParams.y) / _ScreenParams.y);
        float2 move = float2((1 / _ScreenParams.x), (1 / _ScreenParams.y));

        half depth = 1 - max(abs(col.x), max(abs(col.y), abs(col.z)));

        //main loop
        for (int j = 1; j < _LineWidth; j++)
        {
            for (int i = 3; i >= 1; i--)
            {
                float2 nghuv = SampleNeighbour(i, rounduv, move * j);
                half4 nghCol = tex2D(_GrabTex, nghuv);
                //reformat
                nghCol = nghCol * 2 - 1;

                half nghDepth = 1 - max(abs(nghCol.x), max(abs(nghCol.y), abs(nghCol.z)));

                //check
                //return dot(col, nghCol) / length(col) / length(nghCol);
                //half normalDiff = abs(dot(col, nghCol)) / (length(col) * length(nghCol));
                half normalDiff = abs(col.x * nghCol.x + col.y * nghCol.y + col.z * nghCol.z) / (length(col) * length(nghCol));
                if (normalDiff < _NormalsThreshold)
                //if (normalDiff < 50)
                {
                    return half4(1, 1, 1, 1);
                }
                //depth:
                half depthDiff = abs(depth - nghDepth);
                //*
                if (depthDiff > _DepthThreshold)
                {
                    return half4(1, 1, 1, 1);
                }
                //*/
            }
        }
        //*/

        return half4(0, 0, 0, 1);
        return col;
    }
    ENDCG
    }

        }
    }
