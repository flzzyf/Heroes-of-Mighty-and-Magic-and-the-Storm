﻿Shader "UnlitShadows/UnlitShadowCast" {
Properties{ _MainTex("Base (RGB)", 2D) = "white" {} }
SubShader{ Pass{ SetTexture[_MainTex] } } FallBack "VertexLit" }