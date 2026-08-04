// Bends the finished frame outward from the middle, so the menu reads as a curved
// surface seen head on rather than a flat one.
//
// The maths is the usual radial polynomial: a pixel at radius r from the centre is
// filled with whatever was at r * (1 + strength * r squared), so the middle is
// magnified and the outside is compressed into less room than it had. Straight lines
// bow, which is the whole point.
//
// Two details are not the usual ones.
//
// The radius is corrected for aspect before it is squared, so the curve is circular
// on a 4:3 window and stays circular when the window is not 4:3. Without that the
// screen bulges more vertically than horizontally and reads as a squashed lens.
//
// And the whole displacement is divided by its own value at the corner, which pins
// the corners exactly where they were. Otherwise a positive strength samples past the
// edge of the frame out there and returns black, and the effect arrives wearing four
// dark triangles.
Shader "Soulbound/MenuFisheye" {
    Properties {
        _MainTex   ("Base", 2D) = "white" {}
        _Strength  ("Strength", Range(-0.9, 0.9)) = 0.25
        _Vignette  ("Vignette", Range(0, 1)) = 0.28
        _Aspect    ("Aspect", Float) = 1.3333
    }

    SubShader {
        Cull Off
        ZWrite Off
        ZTest Always

        Pass {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Strength;
            float _Vignette;
            float _Aspect;

            fixed4 frag(v2f_img i) : SV_Target {
                float2 c = i.uv * 2.0 - 1.0;

                // Radius squared, aspect corrected and scaled so it is 1 at the corner.
                float2 a = float2(c.x * _Aspect, c.y);
                float r2 = dot(a, a) / (_Aspect * _Aspect + 1.0);

                float2 uv = c * (1.0 + _Strength * r2) / (1.0 + _Strength) * 0.5 + 0.5;

                // Nothing outside the frame exists to be shown. The corners are pinned so
                // this should not trigger, but a negative strength or a hand edited value
                // can reach past the edge and smeared edge pixels look like a bug.
                if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0)
                    return fixed4(0.0, 0.0, 0.0, 1.0);

                fixed4 col = tex2D(_MainTex, uv);
                col.rgb *= 1.0 - _Vignette * r2;
                return col;
            }
            ENDCG
        }
    }

    Fallback Off
}
