﻿// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Particles/Standard Unlit No ZWrite"
{
	Properties
	{
		_MainTex("Albedo", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)

		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}

		_DistortionStrength("Strength", Float) = 1.0
		_DistortionBlend("Blend", Range(0.0, 1.0)) = 0.5

		_SoftParticlesNearFadeDistance("Soft Particles Near Fade", Float) = 0.0
		_SoftParticlesFarFadeDistance("Soft Particles Far Fade", Float) = 1.0
		_CameraNearFadeDistance("Camera Near Fade", Float) = 1.0
		_CameraFarFadeDistance("Camera Far Fade", Float) = 2.0

		[HideInInspector] _Mode("__mode", Float) = 0.0
		[HideInInspector] _ColorMode("__colormode", Float) = 0.0
		[HideInInspector] _FlipbookMode("__flipbookmode", Float) = 0.0
		[HideInInspector] _LightingEnabled("__lightingenabled", Float) = 0.0
		[HideInInspector] _DistortionEnabled("__distortionenabled", Float) = 0.0
		[HideInInspector] _EmissionEnabled("__emissionenabled", Float) = 0.0
		[HideInInspector] _BlendOp("__blendop", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
		[HideInInspector] _Cull("__cull", Float) = 2.0
		[HideInInspector] _SoftParticlesEnabled("__softparticlesenabled", Float) = 0.0
		[HideInInspector] _CameraFadingEnabled("__camerafadingenabled", Float) = 0.0
		[HideInInspector] _SoftParticleFadeParams("__softparticlefadeparams", Vector) = (0,0,0,0)
		[HideInInspector] _CameraFadeParams("__camerafadeparams", Vector) = (0,0,0,0)
		[HideInInspector] _ColorAddSubDiff("__coloraddsubdiff", Vector) = (0,0,0,0)
		[HideInInspector] _DistortionStrengthScaled("__distortionstrengthscaled", Float) = 0.0
	}

		Category
		{
			SubShader
			{
				Tags { "RenderType" = "Opaque" "IgnoreProjector" = "True" "PreviewType" = "Plane" "PerformanceChecks" = "False" }

				Cull Off
				Lighting Off
				ZWrite Off
				ZTest Always
				Blend SrcAlpha OneMinusSrcAlpha

				Pass
				{
					Tags { "LightMode" = "ForwardBase" }

					CGPROGRAM

					#pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON
					#pragma shader_feature_local _ _COLOROVERLAY_ON _COLORCOLOR_ON _COLORADDSUBDIFF_ON

					#pragma vertex vertParticleUnlit
					#pragma fragment fragParticleUnlit
					#pragma multi_compile_instancing
					#pragma instancing_options procedural:vertInstancingSetup

					#include "UnityStandardParticles.cginc"
					ENDCG
				}
			}
		}

			Fallback "VertexLit"
			CustomEditor "StandardParticlesShaderGUI"
}