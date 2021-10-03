Shader "LD49/SpriteDistortion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_DistEffect1 ("Distortion Effect", Range(0,1)) = 1
		_Dist1Tex ("Distortion Texture 1", 2D) = "bump" {}
		//_ScreenSpaceDist ("Screen Space UVs", Range(0,1)) = 0
		_DistStrength ("Distortion Strength (xy=dist1, zw=dist2)", vector) = (0.1, 0.1, 0.1, 0.1)
		_ScrollSpeedDist ("Distortion Scroll Speed (xy=dist1, zw=dist2)", vector) = (0, 0, 0, 0)
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
		Cull Off
		ZWrite [_ZWriteMode]
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

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
			float _DistEffect1;
			sampler2D _Dist1Tex;
			float4 _Dist1Tex_ST;
			float4 _ScrollSpeedDist;
			float4 _DistStrength;
			//fixed _ScreenSpaceDist;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);

				o.uvDist1 = TRANSFORM_TEX((v.uv.xy), _Dist1Tex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed2 distAmount = 0;
				//float2 uvScrnDist = TRANSFORM_TEX(ScreenSpaceCoordinate, _Dist1Tex);

				float2 scrollSpeedDist1 = _ScrollSpeedDist.xy ;
				//i.uvDist1 = lerp(i.uvDist1, uvScrnDist, _ScreenSpaceDist);
				i.uvDist1 += scrollSpeedDist1 * _Time.x * _Dist1Tex_ST;
				fixed4 dist1Col = tex2D(_Dist1Tex, i.uvDist1);

				fixed dist1StrengthX = _DistStrength.x;
				fixed dist1StrengthY = _DistStrength.y;
				fixed2 dist1Strength = 0;
				dist1Strength.r = (dist1Col.r - 0.5)*2;
				dist1Strength.g = (dist1Col.g - 0.5)*2;
				distAmount.x = dist1Strength.r * (dist1StrengthX/1);
				distAmount.y = dist1Strength.g * (dist1StrengthY/1);
                fixed4 col = tex2D(_MainTex, (i.uv + (distAmount * _DistEffect1)));
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
