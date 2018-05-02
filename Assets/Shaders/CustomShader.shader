Shader "GUI/Textured Text Alpha Shader" {
 
 Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}
       
 Category {
    Lighting Off
    Blend SrcAlpha OneMinusSrcAlpha
    AlphaTest Greater .01
    ColorMask RGBA
    cull off
    ztest always
    Zwrite off
    Fog { Mode Off }
   
    Tags {"Queue" = "Transparent" "RenderMode" = "Transparent"}
   
    BindChannels {
      Bind "Color", color
    }
   
    // ---- Dual texture cards
    SubShader {
        Pass {

            SetTexture [_MainTex] {
				constantColor [_Color]
                combine texture * primary  // previous
            }
           
            SetTexture [_MainTex] {
                constantColor [_Color]
                combine constant * previous
            }

        }
    }
   
    // ---- Single texture cards (does not do color tint)
    SubShader {
        Pass {
            SetTexture [_MainTex] {
                combine texture * primary
            }
        }
    }

	}
 }