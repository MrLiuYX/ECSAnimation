// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/HpBar"
{
    Properties
    {
        _DataTex ("DataTex", 2D) = "white" {}
        _BoardCol("BoardCol", color) = (0.1,0.1,0.1,1)
        _XBoardLen("XBoardLen", range(0, 1)) = 0.05
        _YBoardLen("YBoardLen", range(0, 1)) = 0.05
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_instancing 
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                uint vertexID : SV_VERTEXID;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 col : TEXCOORD2;
                float4 data2 : TEXCOORD3;
            };

            //C#
            float _Size;

            //Shader
            sampler2D _DataTex;
            float4 _DataTex_ST;
            fixed4 _BoardCol;
            float _XBoardLen;
            float _YBoardLen;

            v2f vert (appdata v, uint id : SV_INSTANCEID)
            {
                v2f o;
                o.uv = v.uv;
                //Èý¸öÏñËØµã
                id *= 3;

                float normalizeLen = (1 / _Size);
                float4 data1 = tex2Dlod(_DataTex, float4(id % _Size, id / _Size, 0, 0) * normalizeLen);                
                float4 data2 = tex2Dlod(_DataTex, float4((id + 1) % _Size, (id + 1) / _Size, 0, 0) * normalizeLen);             
                float4 data3 = tex2Dlod(_DataTex, float4((id + 2) % _Size, (id + 2) / _Size, 0, 0) * normalizeLen);      

                v.vertex *= float4(data2.z, data2.w, 1, 1);
                o.vertex = mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_MV, float4(data3.x, data1.w, data3.z, 1)) + float4(v.vertex.x, v.vertex.y, 0, 0));
                o.col = fixed4(data1.xyz, 1);
                o.data2 = data2;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {                
                bool isBoard = false;
                isBoard = isBoard 
                || i.uv.x - _XBoardLen <= 0
                || i.uv.x + _XBoardLen >= 1
                || i.uv.y - _YBoardLen <= 0
                || i.uv.y + _YBoardLen >= 1;

                fixed4 col = 1;
                if(isBoard)
                {
                    col = _BoardCol;
                }
                else
                {
                    float ratio = i.data2.x / i.data2.y;
                    if (i.uv.x <= ratio) 
                    {
                        col = i.col;
                    }
                    else
                    {
                        col = fixed4(0,0,0,1);
                    }
                }

                return col;
            }
            ENDCG
        }
    }
}
