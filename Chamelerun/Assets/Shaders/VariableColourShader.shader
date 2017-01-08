Shader "Chamelerun/VariableColourShader"
{
	Properties
	{
		[MaterialToggle] _UseFog("Use Fog", Float) = 1
		_AmbientInfluence("Ambient Influence", Range(0, 1)) = 0.5
		_LightInfluence("Light Influence", Range(0, 1)) = 0.5
		_LightMap ("LightMap", 2D) = "white" {}
		_MainShadows("Shadows", Color) = (0, 0, 0, 1)
		_MainMidtones("Midtones", Color) = (0.5, 0.5, 0.5, 1)
		_MainHighlights("Highlights", Color) = (1, 1, 1, 1)

		[Space(50)]_DetailMapA("PrimaryDetails", 2D) = "white"{}
		_DetailAShadows("Shadows", Color) = (0, 0, 0, 1)
		_DetailAMidtones("Midtones", Color) = (0.5, 0.5, 0.5, 1)
		_DetailAHighlights("Highlights", Color) = (1, 1, 1, 1)

		[Space(50)]_DetailMapB("SecondaryDetails", 2D) = "white"{}
		_DetailBShadows("Shadows", Color) = (0, 0, 0, 1)
		_DetailBMidtones("Midtones", Color) = (0.5, 0.5, 0.5, 1)
		_DetailBHighlights("Highlights", Color) = (1, 1, 1, 1)

		[Space(50)]_DetailMapC("TertiaryDetails", 2D) = "white"{}
		_DetailCShadows("Shadows", Color) = (0, 0, 0, 1)
		_DetailCMidtones("Midtones", Color) = (0.5, 0.5, 0.5, 1)
		_DetailCHighlights("Highlights", Color) = (1, 1, 1, 1)
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "LightMode" = "ForwardBase" }
		LOD 100

		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vertexShader
			#pragma fragment fragmentShader
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase 

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			fixed luminance(fixed3 color);
			fixed3 interpolate(fixed3 a, fixed3 b, fixed factor);
			fixed4 blend(fixed4 bg, fixed4 fg);
			fixed3 gradientMap(fixed factor, fixed3 shadows, fixed3 midtones, fixed3 highlights);

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float3 vertexLighting : TEXCOORD1;
			};

			float _UseFog;
			float _AmbientInfluence;
			float _LightInfluence;

			sampler2D _LightMap;
			float4 _LightMap_ST;

			sampler2D _DetailMapA;
			float4 _DetailMapA_ST;

			sampler2D _DetailMapB;
			float4 _DetailMapB_ST;

			sampler2D _DetailMapC;
			float4 _DetailMapC_ST;

			float4 _MainShadows;
			float4 _MainMidtones;
			float4 _MainHighlights;

			float4 _DetailAShadows;
			float4 _DetailAMidtones;
			float4 _DetailAHighlights;

			float4 _DetailBShadows;
			float4 _DetailBMidtones;
			float4 _DetailBHighlights;

			float4 _DetailCShadows;
			float4 _DetailCMidtones;
			float4 _DetailCHighlights;
			
			v2f vertexShader(appdata input)
			{
				v2f output;
				output.pos = UnityObjectToClipPos(input.vertex);
				output.uv = TRANSFORM_TEX(input.uv, _LightMap);

				float3 vertexLighting;

				for (int index = 0; index < 4; index++)
				{	   
					float3 vertexToLightSource = float3(unity_4LightPosX0[index], unity_4LightPosY0[index], unity_4LightPosZ0[index]) - 
						mul(unity_ObjectToWorld, input.vertex);
					vertexLighting += 1.0 / (1.0 + unity_4LightAtten0[0] * dot(vertexToLightSource, vertexToLightSource)) * 
						unity_LightColor[index];
				}

				output.vertexLighting = vertexLighting;
				return output;
			}
			
			fixed4 fragmentShader(v2f input) : SV_Target
			{
				fixed4 lightmap = tex2D(_LightMap, input.uv);
				if (lightmap.a == 0)
				{
					return lightmap;
				}
				fixed lightness = luminance(lightmap);
				fixed4 color = fixed4(gradientMap(lightness, _MainShadows, _MainMidtones, _MainHighlights), lightmap.a);

				if(_DetailAMidtones.a != 0)
				{
					fixed4 detailA = tex2D(_DetailMapA, input.uv);
					if (detailA.a != 0 && detailA.r != 1)
					{
						color = blend(color, 
							fixed4(gradientMap(lightness, _DetailAShadows, _DetailAMidtones, _DetailAHighlights), 
							1 - luminance(detailA)));
					}
				}

				if(_DetailBMidtones.a != 0)
				{
					fixed4 detailB = tex2D(_DetailMapB, input.uv);
					if (detailB.a != 0 && detailB.r != 1)
					{
						color = blend(color, 
							fixed4(gradientMap(lightness, _DetailBShadows, _DetailBMidtones, _DetailBHighlights),
							1 - luminance(detailB)));
					}
				}

				if (_DetailCMidtones.a != 0)
				{
					fixed4 detailC = tex2D(_DetailMapC, input.uv);
					if (detailC.a != 0 && detailC.r != 1)
					{
						color = blend(color,
							fixed4(gradientMap(lightness, _DetailCShadows, _DetailCMidtones, _DetailCHighlights),
								1 - luminance(detailC)));
					}
				}

				if (_AmbientInfluence != 0 && lightness != 1)
				{
					color = fixed4(interpolate(interpolate(color, unity_AmbientSky, _AmbientInfluence), color, lightness), lightmap.a);
				}

				if (_LightInfluence != 0 && lightness != 0)
				{
					fixed lightAlpha = luminance(input.vertexLighting);
					if (lightAlpha != 0)
					{
						color = blend(color, fixed4(input.vertexLighting, _LightInfluence * lightness * lightAlpha));
					}
				}

				if(_UseFog == 1)
				{
					color = fixed4(interpolate(unity_FogColor, color, input.pos.z), color.a);
				}
				return color;
			}

			fixed luminance(fixed3 color)
			{
				return ((color.r + color.r + color.b + color.g + color.g + color.g) / 6);
			}

			fixed3 interpolate(fixed3 a, fixed3 b, fixed factor)
			{
				return a * (1- factor) + b * factor;
			}

			fixed4 blend(fixed4 bg, fixed4 fg)
			{
				fixed a1 = 1 - (1 - fg.a) * (1 - bg.a);
				fixed a2 = bg.a * (1 - fg.a);
				return fixed4((fg.r * fg.a + bg.r * a2) / a1,
					(fg.g * fg.a + bg.g * a2) / a1,
					(fg.b * fg.a + bg.b * a2) / a1, bg.a);
			}

			fixed3 gradientMap(fixed factor, fixed3 shadows, fixed3 midtones, fixed3 highlights)
			{
				if (factor == 0)
				{
					return shadows;
				}
				if (factor == 1)
				{
					return highlights;
				}

				if (factor < 0.5)
				{
					return interpolate(shadows, midtones, factor * 2);
				}
				else
				{
					return interpolate(midtones, highlights, (factor - 0.5) * 2);
				}
			}
			
			ENDCG
		}
	}
}
