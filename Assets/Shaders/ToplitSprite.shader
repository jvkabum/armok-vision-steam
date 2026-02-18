// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/ToplitSprite" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
        _SpriteArray("Sprite Array", 2DArray) = "white" {}
        _NormalArray("Normal Array", 2DArray) = "bump" {}
        _SpriteIndex("Sprite Index", Int) = 1
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="TransparentCutout" }
		LOD 200
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard addshadow alphatest:_Cutoff

        #pragma target 3.5

        UNITY_DECLARE_TEX2DARRAY(_ImageAtlas0);
        UNITY_DECLARE_TEX2DARRAY(_ImageAtlas1);
        UNITY_DECLARE_TEX2DARRAY(_ImageAtlas2);
        UNITY_DECLARE_TEX2DARRAY(_ImageAtlas3);
        UNITY_DECLARE_TEX2DARRAY(_ImageBumpAtlas0);
        UNITY_DECLARE_TEX2DARRAY(_ImageBumpAtlas1);
        UNITY_DECLARE_TEX2DARRAY(_ImageBumpAtlas2);
        UNITY_DECLARE_TEX2DARRAY(_ImageBumpAtlas3);

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
        int _SpriteIndex;

		void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 layerPixel;
            float3 normal;

            if (_SpriteIndex < 2048) {
                layerPixel = UNITY_SAMPLE_TEX2DARRAY(_ImageAtlas0, float3(IN.uv_MainTex.xy, _SpriteIndex));
                normal = UnpackNormal(UNITY_SAMPLE_TEX2DARRAY(_ImageBumpAtlas0, float3(IN.uv_MainTex.xy, _SpriteIndex)));
            } else if (_SpriteIndex < 4096) {
                layerPixel = UNITY_SAMPLE_TEX2DARRAY(_ImageAtlas1, float3(IN.uv_MainTex.xy, _SpriteIndex - 2048));
                normal = UnpackNormal(UNITY_SAMPLE_TEX2DARRAY(_ImageBumpAtlas1, float3(IN.uv_MainTex.xy, _SpriteIndex - 2048)));
            } else if (_SpriteIndex < 6144) {
                layerPixel = UNITY_SAMPLE_TEX2DARRAY(_ImageAtlas2, float3(IN.uv_MainTex.xy, _SpriteIndex - 4096));
                normal = UnpackNormal(UNITY_SAMPLE_TEX2DARRAY(_ImageBumpAtlas2, float3(IN.uv_MainTex.xy, _SpriteIndex - 4096)));
            } else {
                layerPixel = UNITY_SAMPLE_TEX2DARRAY(_ImageAtlas3, float3(IN.uv_MainTex.xy, _SpriteIndex - 6144));
                normal = UnpackNormal(UNITY_SAMPLE_TEX2DARRAY(_ImageBumpAtlas3, float3(IN.uv_MainTex.xy, _SpriteIndex - 6144)));
            }

            fixed4 c = _Color * layerPixel;
            o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
            o.Normal = normal;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
