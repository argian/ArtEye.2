//WARNING!!! This shader is meant to be unused,
//it only purpose is to serve as a starting point
//to all other "post-processing" shaders
//(the ones that "act" as such in vrChat)
Shader "Trash/ImageEffectTemplate"
{
    Properties
    {
        
    }
    SubShader
    {
        // Draw after all opaque geometry
        Tags {
            "RenderType" = "Transparent"
            "Queue" = "Geometry+1" //change it to correspond to scene!
            "LightMode" = "Always"
            "IgnoreProjector" = "True"
            "DisableBatching" = "True"
            "ForceNoShadowCasting" = "True"
            "PreviewType" = "Plane"
            "VRCFallback" = "Hidden"
        }

        ZWrite Off
        ZTest Always
        Cull Off

        // Grab the screen behind where object is rendered
        GrabPass
        {
            "_GrabTex"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 clipPos : SV_POSITION;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                //this magic gets screen uv
                o.clipPos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.clipPos);
                return o;
            }

sampler2D _GrabTex;

            half4 frag(v2f i) : SV_Target
            {
                float2 uv = i.grabPos.xy / i.grabPos.w;
                half4 col = tex2D(_GrabTex, uv);
                //calculate screen col and uv and proceed to do nothing
                return col;
            }
            ENDCG
        }
    }
}
