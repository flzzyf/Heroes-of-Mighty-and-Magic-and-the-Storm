Shader "UnlitShadows/UnlitShadowCastCutout" {
Properties{ _Color("Main Color", Color) = (1,1,1,1) _MainTex("Base (RGB)", 2D) = "white" {} _Cutoff("Cutout", Range(0,1)) = 0.5 }
SubShader{ Pass{ Alphatest Greater[_Cutoff] SetTexture[_MainTex] } } Fallback "Transparent/Cutout/VertexLit" }