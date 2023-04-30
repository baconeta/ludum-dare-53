Shader "Custom/RiverWater" {
    Properties {
        //Texture Maps
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        
        //ColourGradient
        //Main Colour Gradients Cutoffs are 0-Dark 1-Bright
        _MainColourGradients ("Main Colour Gradient", Vector) = (0, 0, 0, 0)
        _MainColourBright ("Main Colour Bright", Color) = (0, 0, 0, 0)
        _MainColourLight ("Main Colour Light", Color) = (0, 0, 0, 0)
        _MainColourMid ("Main Colour Mid", Color) = (0, 0, 0, 0)
        _MainColourDark ("Main Colour Dark", Color) = (0, 0, 0, 0)
        
        //River Parameters
        _FlowDirection ("Flow Direction", Vector) = (0, -1, 0, 0)
        _Speed ("Speed", Range(0, 10)) = 1
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 1
        _WaveAmplitude ("Wave Amplitude", Range(0, 1)) = 0.1
        
        //Util
        _Pause("Pause", Range(0,1)) = 0
    }

    SubShader {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        sampler2D _BumpMap;

        //Colour Gradient
        float4 _MainColourGradients;
        float4 _MainColourBright;
        float4 _MainColourLight;
        float4 _MainColourMid;
        float4 _MainColourDark;

        //River Parameters
        float2 _FlowDirection;
        float _Speed;
        float _WaveSpeed;
        float _WaveAmplitude;

        //Util
        int _Pause;
        float4 _CustomTime;


        void surf (float2 uv_MainTex,float3 worldPos, inout SurfaceOutput output) {

            float2 uv = uv_MainTex;

            // Is Paused
            if(_Pause == 1)
            {
                // Calculate the texture coordinate offset based on flow direction and time
                uv += _FlowDirection * _CustomTime.y * _Speed;
                
                // Add a time-based animation to the texture coordinate offset to make the texture move
                uv += float2(0, sin(_CustomTime.y * _WaveSpeed + worldPos.y) * _WaveAmplitude);
            }
            else //Not paused
            {
                //Increment Time
                _CustomTime += _Time;

                //Movement
                // Calculate the texture coordinate offset based on flow direction and time
                uv += _FlowDirection * _CustomTime.y * _Speed;

                //Wave Animation
                // Add a time-based animation to the texture coordinate offset to make the texture move
                uv += float2(0, sin(_CustomTime.y * _WaveSpeed + worldPos.x) * _WaveAmplitude);
            }
            

            // Sample the normal map at the uv coordinate.
            float normal = tex2D(_BumpMap, uv);

            // Calculate the height based on the sampled noise
            float height = normal.r;

            // Set the surface output properties
            output.Albedo = _MainColourBright + height;
            output.Alpha = 1;

            // Apply normal mapping to create the illusion of waves
            output.Normal = UnpackNormal(tex2D(_BumpMap, worldPos.xy * 0.01).rgba);
        }
        ENDCG
    }
    FallBack "Diffuse"
}