// Shader that is invisible but casts shadows.
Shader "Ultimate Character Controller/First Person Controller/Invisible Shadow Caster" {
	SubShader{
		Tags {
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			// Do the minimal amount of work because the object isn't going to be seen.
			struct v2f {
				half4 pos : SV_POSITION;
			};

			v2f vert(appdata_img v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			half4 frag(v2f_img i) : COLOR
			{
				return half4(0,0,0,0);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}