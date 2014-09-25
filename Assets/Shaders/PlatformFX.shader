Shader "Custom/PlatformFX" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 1)
		_GlowThresh ("Max Glow Alpha Threshold", Range(0,0.5)) = 0.375
		_PulseSpeed ("Pulse Speed", Range(0, 4)) = 2
		_AlphaTex ("Precomputed Glow Map", 2D) = "white" {}
	}
	SubShader {
		Tags
		{
			"RenderType"="Transparent"
			"Queue" = "Transparent"
		}
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		LOD 200
		CGPROGRAM
		#pragma surface surf Lambert
		struct Input {
			float2 uv_AlphaTex;
		};
		float4 _Color;
		float _GlowThresh;
		float _PulseSpeed;
		sampler2D _AlphaTex;
		
		float4 neonColor(float2 uv)
		{
			float a = tex2D(_AlphaTex, uv).a;
			
			//approx of triangle wave pulled from wikipedia			
			float thresh = _GlowThresh;
			float4 c = (0,0,0, (a-thresh<=0)?0:a);
			
			//strictly speaking the addition of the alpha stuff here
			//doesn't matter.  It looks cooler though.
			c.rgb = _Color.rgb + max(min(a-thresh,1), 0);
			return c;
		}
		
		void surf (Input IN, inout SurfaceOutput o) {
			float2 uv = IN.uv_AlphaTex;
			
			// get the color from the neon "glow"
			float4 c = neonColor(uv);
					
			// discard the pixel if it can't be seen.  Since colored portions of the texture probably
			// do not fill it, this allows a rotation + intersection of different quads to not look
			// weird.
			if(c.a == 0)
			{
				discard;
			}
			else if(c.a > 0.75)
			{
				//make the central portions of light more intense.
				c.rgb *= (1+c.a);
			}
			
			o.Emission = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Transparent/Vertex"
}
