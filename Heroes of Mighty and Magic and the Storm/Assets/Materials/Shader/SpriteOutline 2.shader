Shader "Custom/Sprite Shadow" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.5
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
        Lighting Off
        ZWrite Off
 
        CGPROGRAM
        #pragma surface surf Lambert addshadow alphatest:_Cutoff
			#include "UnityCG.cginc"

		sampler2D _MainTex;
		
        struct Input
        {
            float2 uv_MainTex;
        };
 
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}