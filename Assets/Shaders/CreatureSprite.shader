// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/CreatureSprite" {
	Properties {
        _MatTex("Albedo (RGB)", 2DArray) = "gray" {}
        _BumpMap("BumpMap", 2DArray) = "bump" {}
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
			float2 uv_MatTex;
		};

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _LayerColor)
#define _LayerColor_arr Props
            UNITY_DEFINE_INSTANCED_PROP(float, _LayerIndex)
#define _LayerIndex_arr Props
        UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
            float layerIndex = UNITY_ACCESS_INSTANCED_PROP(_LayerIndex_arr, _LayerIndex);
            fixed4 layerPixel;
            float3 normal;

            if (layerIndex < 2048) {
                layerPixel = UNITY_SAMPLE_TEX2DARRAY(_ImageAtlas0, float3(IN.uv_MatTex.xy, layerIndex));
                normal = UnpackNormal(UNITY_SAMPLE_TEX2DARRAY(_ImageBumpAtlas0, float3(IN.uv_MatTex.xy, layerIndex)));
            } else if (layerIndex < 4096) {
                layerPixel = UNITY_SAMPLE_TEX2DARRAY(_ImageAtlas1, float3(IN.uv_MatTex.xy, layerIndex - 2048));
                normal = UnpackNormal(UNITY_SAMPLE_TEX2DARRAY(_ImageBumpAtlas1, float3(IN.uv_MatTex.xy, layerIndex - 2048)));
            } else if (layerIndex < 6144) {
                layerPixel = UNITY_SAMPLE_TEX2DARRAY(_ImageAtlas2, float3(IN.uv_MatTex.xy, layerIndex - 4096));
                normal = UnpackNormal(UNITY_SAMPLE_TEX2DARRAY(_ImageBumpAtlas2, float3(IN.uv_MatTex.xy, layerIndex - 4096)));
            } else {
                layerPixel = UNITY_SAMPLE_TEX2DARRAY(_ImageAtlas3, float3(IN.uv_MatTex.xy, layerIndex - 6144));
                normal = UnpackNormal(UNITY_SAMPLE_TEX2DARRAY(_ImageBumpAtlas3, float3(IN.uv_MatTex.xy, layerIndex - 6144)));
            }

            fixed4 layerColor = UNITY_ACCESS_INSTANCED_PROP(_LayerColor_arr, _LayerColor);

            o.Albedo = layerPixel.rgb * layerColor.rgb;
            o.Metallic = max((layerColor.a * 2) - 1, 0);
            o.Alpha = layerPixel.a;
            o.Normal = normal;
		}
		ENDCG
	}
	FallBack "Diffuse"
    //CustomEditor "CreatureSpriteEditor"

}
