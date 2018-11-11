Shader "Custom/Sprite Outline" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _LineWidth("LineWidth", Range(0, 30)) = 10
        _Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.5
    }
    //描边
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "IgnoreProjector"="True"
            "RenderType"="TransparentCutout"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        LOD 200
        Cull Off
        Lighting On
        Blend One OneMinusSrcAlpha
        ZWrite Off

        Pass {
        Lighting On
 			ZTest Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag 
            #include "UnityCG.cginc"
 
            sampler2D _MainTex;
            float _LineWidth;
  
            fixed4 _Color;
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
                half4 outlineC = _Color;
                outlineC.a *= ceil(c.a);
                outlineC.rgb *= outlineC.a;
 
                fixed alpha_up = tex2D(_MainTex, i.uv + fixed2(0, _MainTex_TexelSize.y * _LineWidth)).a;
                fixed alpha_down = tex2D(_MainTex, i.uv - fixed2(0, _MainTex_TexelSize.y * _LineWidth)).a;
                fixed alpha_right = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x * _LineWidth, 0)).a;
                fixed alpha_left = tex2D(_MainTex, i.uv - fixed2(_MainTex_TexelSize.x * _LineWidth, 0)).a;
 
                return lerp(outlineC, c, ceil(alpha_up * alpha_down * alpha_right * alpha_left));
            }  

            ENDCG
        }
    }
//阴影
    SubShader
    {
        Tags
        {
            "Queue"="AlphaTest"
            "IgnoreProjector"="True"
            "RenderType"="TransparentCutout"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
 
        LOD 200
        Cull Off
        Lighting On
        ZWrite Off
 
        CGPROGRAM
        #pragma surface surf Lambert addshadow alphatest:_Cutoff
 
        sampler2D _MainTex;
        fixed4 _Color;
 
        struct Input
        {
            float2 uv_MainTex;
        };
 
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}