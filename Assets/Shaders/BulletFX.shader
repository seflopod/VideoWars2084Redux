Shader "Custom/BulletFX" {
	Properties {
		_Color ("Color", Color) = (1, 1, 1, 1)
		_GlowThresh ("Max Glow Alpha Threshold", Range(0,0.5)) = 0.375
		_PulseSpeed ("Pulse Speed", Range(0.1, 4)) = 2
		_AlphaTex ("Precomputed Glow Map", 2D) = "white" {}
		_InnerTex ("Interior Texture", 2D) = "white" {}
		_InnerAlpha ("Interior Transparency", Range(0,1)) = 0.125
		_InnerShift ("Inner Hue Shift", Float) = 60
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
			float3 worldPos;
		};
		const float PI = 3.1415926;
		const float MAX_DELTA_H = 30;
		float4 _Color;
		float _GlowThresh;
		float _PulseSpeed;
		sampler2D _AlphaTex;
		sampler2D _InnerTex;
		float _InnerAlpha;
		float _InnerShift;
		
		// http://beesbuzz.biz/code/hsv_color_transforms.php
		// dH is in degrees
		float3 rotateHSV(float3 c, float dH)
		{
			float u = cos(radians(dH));
			float w = sin(radians(dH));
			float3x3 rotMat = float3x3(0.299+0.701*u+0.168*w, 0.587-0.587*u+0.330*w, 0.114-0.114*u-0.497*w,
										0.299-0.299*u-0.328*w, 0.587+0.413*u+0.035*w, 0.114-0.114*u+0.292*w,
										0.299-0.299*u+1.250*w, 0.587-0.587*u-1.050*w, 0.114+0.886*u-0.203*w);
			float3 ret = saturate(mul(rotMat, c));
			return ret;
		}
		
		float4 fillColor(float2 uv)
		{
			float4 c = tex2D(_InnerTex, uv) * _Color;
			c.rgb = rotateHSV(c.rgb, clamp(_InnerShift, 0, 360));
			//c.a = 1;
			c.a = 1-_InnerAlpha;
			return c;
		}
		
		float4 neonColor(float2 uv)
		{
			float a = tex2D(_AlphaTex, uv).a;
			
			//approx of triangle wave pulled from wikipedia
			float dist = abs(4*frac(_PulseSpeed * _Time.y/2*PI-0.25) - 2) - 1;
			
			//float thresh = lerp(_GlowThresh, 0.5, min(dist,1));
			float thresh = _GlowThresh;
			float4 c = (0,0,0, (a-thresh<=0)?0:a);
			
			//strictly speaking the addition of the alpha stuff here
			//doesn't matter.  It looks cooler though.
			c.rgb = _Color.rgb + max(min(a-thresh,1), 0);
			return c;
		}
		
		float softBlend(float base, float top)
		{
			return (1-2*base)*top*top + 2*base*top;
		}
		
		void surf (Input IN, inout SurfaceOutput o) {
			float2 uv = IN.uv_AlphaTex;
			
			// get the color of the fill
			float4 fill = fillColor(uv);
			
			// get the color from the neon "glow"
			float4 neon = neonColor(uv);
			
			// blend the two colors from above
			float4 c = 0;
			if(fill.r != 0 || fill.g !=0 || fill.b != 0)
			{
				for(int i=0;i<3;i++)
				{
					// blend the r, g, or b channel with the neon version of the same
					// weighting based on the alpha of the neon color
					c[i] = fill[i] + softBlend((1-neon.a) * fill[i], neon.a*neon[i]);
				}
				
				c.a = max(_InnerAlpha, neon.a);
			}
			else
			{
				c = neon;
			}
			
			// discard the pixel if it can't be seen.  Since colored portions of the texture probably
			// do not fill it, this allows a rotation + intersection of different quads to not look
			// weird.
			if(c.a == 0)
			{
				discard;
			}
			
			o.Emission = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Transparent/Vertex"
}
