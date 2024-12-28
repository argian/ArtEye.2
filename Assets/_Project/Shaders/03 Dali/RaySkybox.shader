Shader "ArtEye/RaySkybox"
{
    // The properties block of the Unity shader. In this example this block is empty
    // because the output color is predefined in the fragment shader code.
    Properties
    { 
        LightDir("LightDir", vector) = (0, 0, 0, 1)
        FacingDir("FacingDir", vector) = (0, 0, 1, 1)
        CubeCol("CubeCol", Color) = (0, 0, 0, 0)
        CubeCol2("CubeCol2", Color) = (0, 0, 0, 0)
        CubeCol3("CubeCol3", Color) = (0, 0, 0, 0)
        CubeCol4("CubeCol4", Color) = (0, 0, 0, 0)
        CubeCol5("CubeCol5", Color) = (0, 0, 0, 0)
        CubeCol6("CubeCol6", Color) = (0, 0, 0, 0)
        BackgruoundUp("BackgruoundUp", Color) = (0, 0, 0, 0)
        BackgruoundDown("BackgruoundDown", Color) = (0, 0, 0, 0)
        MainGroundPos("MainGroundPos", vector) = (0, 0, 0, 0)
        CubeWallPos("CubeWallPos", vector) = (0, 0, 0, 1)
        CubeWallDir("CubeWallDir", vector) = (0, 0, 1, 1)
        CubeWallScale("CubeWallScale", vector) = (0, 0, 0, 1)
        CubeRoofPos("CubeRoofPos", vector) = (0, 0, 0, 1)
        CubeRoofDir("CubeRoofDir", vector) = (0, 0, 1, 1)
        CubeRoofScale("CubeRoofScale", vector) = (0, 0, 0, 1)
    }

        // The SubShader block containing the Shader code.
        SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        ZWrite On
        ZTest LEqual
        Cull Front

        Pass
        {
            HLSLPROGRAM
            // This line defines the name of the vertex shader.
            #pragma vertex vert
            // This line defines the name of the fragment shader.
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            #include "Assets/_Project/Shaders/General/RayTracing.cginc"
            
            struct Attributes
            {

            float4 positionOS   : POSITION;
            };

            struct Varyings
            {
                float4 screenPos : TEXCOORD0;
                float4 clipPos : SV_POSITION;
                float4 ray : TEXCOORD1;
            };

            float3 get_camera_pos()
            {
                float3 worldCam;
                worldCam.x = unity_CameraToWorld[0][3];
                worldCam.y = unity_CameraToWorld[1][3];
                worldCam.z = unity_CameraToWorld[2][3];
                return worldCam;
            }
            // _WorldSpaceCameraPos is broken in VR (single pass stereo)
            static float3 camera_pos = get_camera_pos();

            Varyings vert(Attributes IN)
            {
                // Declaring the output object (OUT) with the Varyings struct.
                Varyings OUT;
                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous clip space.
                //this magic gets screen uv
                OUT.clipPos = TransformObjectToHClip(IN.positionOS.xyz);
                //yeah vr specialties stuff again
                OUT.screenPos = ComputeScreenPos(OUT.clipPos);
                //this thing calculates rayDir accordingly in vr
                float4 worldPos = mul(unity_ObjectToWorld, IN.positionOS);
                OUT.ray.xyz = worldPos.xyz - camera_pos.xyz;
                OUT.ray.w = OUT.clipPos.w;
                return OUT;
            }

            uniform vector Spacing1;
            uniform vector Spacing2;
            uniform vector Spacing3;

            vector LightDir;
            vector FacingDir;
            vector MainGroundPos;
            vector CubeWallPos;
            vector CubeWallDir;
            vector CubeWallScale;
            vector CubeRoofPos;
            vector CubeRoofDir;
            vector CubeRoofScale;
            vector CubeCol;
            vector CubeCol2;
            vector CubeCol3;
            vector CubeCol4;
            vector CubeCol5;
            vector CubeCol6;

            //rayPos is not used, player is fixed as if he is in "center" of world
            //NOTE TO SELF: CHANGE RESULT NORMALS BASED ON FACING DIR
            float4 CubePatternPlane(float3 rayDir, float3 planePos, float3 facingDir, float3 scale, float3 spacing, inout float4 Hit, inout float3 Normals, float4 Col1, float4 Col2)
            {
                //space transformations to make several facing directions
                rayDir = Rotate3DMatrix(rayDir, facingDir);

                //actual object sequence
                if (SimplePlane(rayDir, planePos, scale, Hit))
                {
                    if (SimpleCheckboard(Hit, spacing.xyz))
                    {
                        //col.xyz = half4(0.8, 0.2, 0.6, 1);
                        Normals = Rotate3DMatrix(float3(1, 1, 0), facingDir);
                        Hit.w = 1;
                        return Col1;
                    }
                    else
                    {
                        //return false;
                        //render cube in nearest possible object
                        //            float3 nearestHit = float3((round(Hit.x / spacing.x / 2) * 2 + 0.5 * sign(rayDir.x)) * spacing.x, Hit.y - spacing.y / 2, (round(Hit.z / spacing.z / 2) * 2 + 0.5 * sign(rayDir.z)) * spacing.z);

                        //float3 nearestHole = float3(round((Hit.x + spacing.x / 2) / spacing.x) * spacing.x - spacing.x / 2, Hit.y - spacing.y / 2, );
                        //Hit.z -= planePos.z;
                        //Hit.x -= planePos.x;
                        Hit.xz += spacing.xz * sign(Hit.xz) / 2; //Falty, but will check for now
                        float3 nearestHit = float3((round(Hit.x / spacing.x / 2) * 2 + 0.5 * sign(rayDir.x)) * spacing.x, Hit.y - spacing.y / 2, (round(Hit.z / spacing.z / 2) * 2 + 0.5 * sign(rayDir.z)) * spacing.z);
                        nearestHit.xz -= spacing.xz * sign(Hit.xz) / 2; //+ planePos.xz
                        //nearestHit.xz -= planePos.xz;
                        /*
                        if (nearestHit.x < 0)
                        {
                            nearestHit.x += spacing.x;
                        }
                        */
                        //nearestHit.xz -= planePos.xz % spacing.xz;

                        if (SimpleCube(nearestHit, Hit, rayDir, spacing.xyz / 2, Normals))
                        {
                            Hit.w = 1;
                            return Col2;
                        }

                        //*
                        //okay, we actually have to also check 2 nearest object
                        if (abs(rayDir.x) > abs(rayDir.z))
                        {
                            nearestHit.x += spacing.x * 2 * sign(rayDir.x);
                        }
                        else if (abs(rayDir.z) > abs(rayDir.x))
                        {
                            nearestHit.z += spacing.z * 2 * sign(rayDir.z);
                        }
                        if (SimpleCube(nearestHit, Hit, rayDir, spacing.xyz / 2, Normals))
                        {
                            Hit.w = 1;
                            return Col2;
                        }
                        //*/
                    }
                }

                //cube for testing
                /*
                if (SimpleCube(float3(6, -3, 0), hit, rayDir, float3(2, 2, 2), normals))
                {
                    Hit.w = 1;
                    return true;
                }
                */

                return float4(0, 0, 0, 1);
            }

            vector BackgruoundUp;
            vector BackgruoundDown;

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.screenPos.xy / IN.screenPos.w;

                //well that is here for testing
                half4 col = half4(1, 1, 1, 1);


                float3 rayDir = normalize((IN.ray.xyz / IN.ray.w).xyz);

                //col.xyz = rayDir;
                if (rayDir.y > 0)
                {
                    col.xyz *= BackgruoundUp * VectorAngle(rayDir, float3(0, 1, 0));
                }
                else
                {
                    col.xyz *= BackgruoundDown * VectorAngle(rayDir, float3(0, -1, 0));
                }

                //col.xyz = float4(VectorAngle(rayDir, float3(0, 1, 0)), VectorAngle(rayDir, float3(0, 1, 0)), VectorAngle(rayDir, float3(0, 1, 0)), 0);

                float3 spacing = abs(float3(Spacing1.x, 0.1, Spacing1.z - 400));

                float4 hit = float4(0, 0, 0, 0); //w means if hits or not
                float3 normals = float3(0, 0, 0);

                float4 colTemp = float4(0, 0, 0, 0);
                //main ground
                colTemp = CubePatternPlane(rayDir, MainGroundPos.xyz, FacingDir.xyz, float3(1000, 1, 1000), Spacing1, hit, normals, CubeCol, CubeCol2);
                if (colTemp.w < 0.9)
                {
                    col = colTemp * VectorAngle(rayDir, float3(0, 1, 0));
                }

                //*
                //partial Cube wall 
                colTemp = CubePatternPlane(rayDir, CubeWallPos.xyz, CubeWallDir.xyz, CubeWallScale.xyz, Spacing2.xyz, hit, normals, CubeCol3, CubeCol4);
                if (colTemp.w < 0.9)
                {
                    col = colTemp * VectorAngle(rayDir, float3(0, 1, 0));
                }

                //partial Cube roof 
                colTemp = CubePatternPlane(rayDir, CubeRoofPos.xyz, CubeRoofDir.xyz, CubeRoofScale.xyz, Spacing3.xyz, hit, normals, CubeCol5, CubeCol6);
                if (colTemp.w < 0.9)
                {
                    col = colTemp * VectorAngle(rayDir, float3(0, 1, 0));
                }

                return col;
            }
            ENDHLSL
        }
    }
}