// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Animation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _VertexDataTex ("VertexDataTex", 2D) = "white" {}
        _InstanceDataTex("InstanceDataTex", 2D) = "white" {}
        _UVX ("_UVX", float) = 0
        _Diffuse("Diffuse", Color) = (1, 1, 1, 1) 
        _Size("_Size", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag    
            #pragma multi_compile_fog
            #pragma target 3.5

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                fixed3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                uint vid : SV_VertexID;
                uint sid : SV_INSTANCEID;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed3 worldNormal : TEXCOORD1; 
                float4 debug : TEXCOORD2;
                fixed4 customData : TEXCOORD3;
            };

            sampler2D _MainTex;
            sampler2D _VertexDataTex;
            sampler2D _InstanceDataTex;
            float4 _MainTex_ST;
            float4 _Diffuse;
            float _UVX;
            float _Size;
            float _NoneAnimation;

            v2f vert (appdata v)
            {
                float vid = v.vid + 0.5;
                v.sid *= 3;

                // 读取instanceData        
                float normalizeLen = (1 / _Size);
                float4 instanceData = tex2Dlod(_InstanceDataTex, float4(v.sid % _Size, v.sid / _Size, 0, 0) * normalizeLen);         
                float4 instanceData2 = tex2Dlod(_InstanceDataTex, float4((v.sid + 1) % _Size, (v.sid + 1) / _Size, 0, 0) * normalizeLen);                
                float4 instanceData3 = tex2Dlod(_InstanceDataTex, float4((v.sid + 2) % _Size, (v.sid + 2) / _Size, 0, 0) * normalizeLen);                

                // 顶点位置计算
                float4 vertex = tex2Dlod(_VertexDataTex, float4(vid * _UVX, instanceData.w, 0, 0));
                float4 lastVertex = tex2Dlod(_VertexDataTex, float4(vid * _UVX, instanceData3.x, 0, 0));
                vertex = lerp(lastVertex, vertex, instanceData3.y);

                float4x4 M_Scale = float4x4
                (
                    instanceData2.w,0,0,0,
                    0,instanceData2.w,0,0,
                    0,0,instanceData2.w,0,
                    0,0,0,instanceData2.w
                );
                vertex = mul(M_Scale,vertex);
                float3x3 M_Scale_T = transpose((M_Scale));
                fixed3 rotatedNormal = mul(M_Scale_T, v.normal);

                float4x4 M_rotateX = float4x4
                    (
                    1,0,0,0,
                    0,cos(instanceData2.x),-sin(instanceData2.x),0,
                    0,sin(instanceData2.x),cos(instanceData2.x),0,
                    0,0,0,1
                    );
                float4x4 M_rotateY = float4x4
                    (
                    cos(instanceData2.y),0,sin(instanceData2.y),0,
                    0,1,0,0,
                    -sin(instanceData2.y),0,cos(instanceData2.y),0,
                    0,0,0,1
                    );
                float4x4 M_rotateZ = float4x4
                    (
                        cos(instanceData2.z),-sin(instanceData2.z),0,0,
                        sin(instanceData2.z),cos(instanceData2.z),0,0,
                        0,0,1,0,
                        0,0,0,1
                    );


                vertex = mul(M_rotateX,vertex);
                vertex = mul(M_rotateY,vertex);
                vertex = mul(M_rotateZ,vertex);

                // 计算法线的旋转矩阵的转置
                float3x3 M_rotateX_T = transpose((M_rotateX));
                float3x3 M_rotateY_T = transpose((M_rotateY));
                float3x3 M_rotateZ_T = transpose((M_rotateZ));

                // 将旋转矩阵的转置应用于法线
                rotatedNormal = mul(M_rotateX_T, rotatedNormal);
                rotatedNormal = mul(M_rotateY_T, rotatedNormal);
                rotatedNormal = mul(M_rotateZ_T, rotatedNormal);

                // 归一化法线
                rotatedNormal = normalize(rotatedNormal);

                v2f o;
                o.vertex = UnityObjectToClipPos(vertex + float4(instanceData.rgb, 0));
                o.uv = v.uv;
                o.worldNormal = UnityObjectToWorldNormal(rotatedNormal);
                o.debug = instanceData;
                o.customData = instanceData3;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
                fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz); 
                fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * max(0, dot(worldLight, i.worldNormal));
                fixed3 color = diffuse + ambient; 
                fixed4 col = tex2D(_MainTex, i.uv);
                col += i.customData.zzzz;
                return col;
            }
            ENDCG
        }
    }
}
