Shader "Custom/SphereMask" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_ColorStrength("Color Strength", Range(1, 4)) = 1
		_EmissionColor("Emission Color", Color) = (1,1,1,1)
		_EmissionTex("Emission (RGB)", 2D) = "white" {}
		_EmissionStrength("Emission Strength", Range(0, 4)) = 0
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		//_Position ("WorldPosition", Vector) = (0,0,0,0)
		//_Radius ("Sphere Radius", Range(0, 100)) = 0
		//_Softness("Sphere Softness", Range(0, 100)) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex, _EmissionTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_EmissionTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		half _ColorStrength;
		half _EmissionStrength;
		fixed4 _Color;
		fixed4 _EmissionColor;

		//Global Variables
		float4 GLOBALmask_Position;
		half GLOBALmask_Radius;
		half GLOBALmask_Softness;
		

		UNITY_INSTANCING_BUFFER_START(Props)

		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			//Color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			//Greyscale
			half grayscale = (c.r + c.g + c.b) * 0.333;
			fixed3 c_g = (grayscale);
			//Emission
			fixed4 e = tex2D(_EmissionTex, IN.uv_EmissionTex) * _EmissionColor * _EmissionStrength;
			//SphereMaskCalc
			half d = distance(GLOBALmask_Position, IN.worldPos);
			half sum = saturate((d - GLOBALmask_Radius) / -GLOBALmask_Softness);
			//Lerp
			fixed4 lerpColor = lerp(fixed4(c_g, 1), c * _ColorStrength, sum);
			fixed4 lerpEmission = lerp(fixed4(0, 0, 0, 0), e, sum);

			//Output
			o.Albedo = lerpColor.rgb;
			o.Metallic = _Metallic;
			o.Emission = lerpEmission.rgb;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
