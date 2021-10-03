Shader "LD49/SpriteScrolling"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_ScrollSpeedBase("Scroll Speed", vector) = (0, 0, 0, 0)
		[Enum(Off, 0, On, 1)] _ZWriteMode ("ZWrite", Float) = 0 // default is "Off"
    }
    SubShader
    {
        Tags 
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
        LOD 100
		Cull Back
		ZWrite [_ZWriteMode]
		ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				fixed4 vertexColor : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float2 uvDist1 : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _ScrollSpeedBase;
			//fixed _ScreenSpaceDist;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				i.uv = i.uv += _ScrollSpeedBase * _Time.x * _MainTex_ST;
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
