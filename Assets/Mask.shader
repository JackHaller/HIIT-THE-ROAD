Shader "Custom/Mask" {
	Properties {
		
	}
	SubShader {
		Tags { "Queue" = "Geometry-1" }
		Stencil
		{
		    Ref 1
		    Comp Always
		    Pass Replace
		}
		ColorMask 0
		Zwrite off
		
		CGPROGRAM
		#pragma surface surf Lambert
		

		struct Input {
			float4 color : COLOR;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = (0, 0, 0, 0);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		
		ENDCG
	} 
	FallBack "Diffuse"
}