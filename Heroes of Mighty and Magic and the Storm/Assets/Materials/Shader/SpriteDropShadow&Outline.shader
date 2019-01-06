// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UnityCommunity/Sprites/SpriteDropShadow&Outline"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_ShadowColor("Shadow", Color) = (0,0,0,1)
		_ShadowOffset("ShadowOffset", Vector) = (0,-0.1,0,0)
		_HorizontalShear("HorizontalShear", Range(-1,1)) = 0.5
		_VerticalShear("VerticalShear", Range(-1,1)) = 0.5
		_ScaleY("ScaleY", Range(-1,1)) = 1

		_OutlineColor("OutlineColor", Color) = (1, 1, 1, 1)
		_OutlineWidth("OutlineWidth", Range(0, 30)) = 10

		_Cutoff("Shadow alpha cutoff", Range(0,1)) = 0.5
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

			//投射阴影
			Pass
			{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile _ PIXELSNAP_ON
				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord  : TEXCOORD0;
				};

				fixed4 _Color;
				fixed4 _ShadowColor;
				float4 _ShadowOffset;
				float _HorizontalShear;
				float _VerticalShear;
				float _ScaleY;

				v2f vert(appdata_t IN)
				{
					v2f OUT;

					float4x4 shearTransformationMatrix = float4x4(
						1, _HorizontalShear, 0, 0,
						_VerticalShear, _ScaleY, 0, 0,
						0, 0, 1, 0,
						0, 0, 0, 1);
					
					float4 shearedVertex = mul(shearTransformationMatrix, IN.vertex);
					//OUT.vertex = UnityObjectToClipPos(shearedVertex);

					OUT.vertex = UnityObjectToClipPos(shearedVertex + _ShadowOffset);



					OUT.texcoord = IN.texcoord;
					OUT.color = IN.color *_ShadowColor;
					#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
					
					#endif

					return OUT;
				}

				sampler2D _MainTex;
				sampler2D _AlphaTex;
				float _AlphaSplitEnabled;

				fixed4 SampleSpriteTexture(float2 uv)
				{
					fixed4 color = tex2D(_MainTex, uv);
					color.rgb = _ShadowColor.rgb;

					#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
					if (_AlphaSplitEnabled)
						color.a = tex2D(_AlphaTex, uv).r;
					#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

					return color;
				}

				fixed4 frag(v2f IN) : SV_Target
				{
					fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
					c.rgb *= c.a;
					return c;
				}
			ENDCG
			}

			// draw real sprite
					/*
			Pass
			{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile _ PIXELSNAP_ON 
				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					float2 texcoord  : TEXCOORD0;
				};

				fixed4 _Color;

				v2f vert(appdata_t IN)
				{
					v2f OUT;
					OUT.vertex = UnityObjectToClipPos(IN.vertex);
					OUT.texcoord = IN.texcoord;
					OUT.color = IN.color * _Color;
					#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
					#endif

					return OUT;
				}

				sampler2D _MainTex;
				sampler2D _AlphaTex;
				float _AlphaSplitEnabled;

				fixed4 SampleSpriteTexture(float2 uv)
				{
					fixed4 color = tex2D(_MainTex, uv);

					#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
					if (_AlphaSplitEnabled)
						color.a = tex2D(_AlphaTex, uv).r;
					#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

					return color;
				}

				fixed4 frag(v2f IN) : SV_Target
				{
					fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
					c.rgb *= c.a;
					return c;
				}
			ENDCG
			}
				*/

				//描边
				Pass{
						ZTest Off
						CGPROGRAM
						#pragma vertex vert
						#pragma fragment frag 
						#include "UnityCG.cginc"

						sampler2D _MainTex;
						float _OutlineWidth;
						float _Cutoff;

						fixed4 _OutlineColor;
						float4 _MainTex_TexelSize;

						struct v2f {
							float4 pos : SV_POSITION;
							half2 uv : TEXCOORD0;
						};

						v2f vert(appdata_base v) {
							v2f o;
							o.pos = UnityObjectToClipPos(v.vertex);
							o.uv = v.texcoord;

							return o;
						}

						fixed4 frag(v2f i) : COLOR
						{
							half4 c = tex2D(_MainTex, i.uv);
							c.rgb *= c.a;
							half4 outlineC = _OutlineColor;
							outlineC.a *= ceil(c.a);
							outlineC.rgb *= outlineC.a;

							fixed alpha_up = tex2D(_MainTex, i.uv + fixed2(0, _MainTex_TexelSize.y * _OutlineWidth)).a;
							fixed alpha_down = tex2D(_MainTex, i.uv - fixed2(0, _MainTex_TexelSize.y * _OutlineWidth)).a;
							fixed alpha_right = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x * _OutlineWidth, 0)).a;
							fixed alpha_left = tex2D(_MainTex, i.uv - fixed2(_MainTex_TexelSize.x * _OutlineWidth, 0)).a;

							return lerp(outlineC, c, ceil(alpha_up * alpha_down * alpha_right * alpha_left));
						}

						ENDCG
				}

		}

}