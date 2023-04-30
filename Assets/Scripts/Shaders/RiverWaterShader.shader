Shader "Custom/RiverWater" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _MainColour ("Main Colour", Color) = (0, 0, 0, 0)
        _FlowDirection ("Flow Direction", Vector) = (0, -1, 0, 0)
        _Speed ("Speed", Range(0, 10)) = 1
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 1
        _WaveAmplitude ("Wave Amplitude", Range(0, 1)) = 0.1
        _Pause("Pause", Range(0,1)) = 0
    }

    SubShader {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        sampler2D _BumpMap;
        float4 _MainColour;
        float2 _FlowDirection;
        float _Speed;
        float _WaveSpeed;
        float _WaveAmplitude;
        int _Pause;
        float4 _CustomTime;

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            float2 uv = IN.uv_MainTex;

            // Paused
            if(_Pause == 1)
            {
                // Calculate the texture coordinate offset based on flow direction and time
                uv += _FlowDirection * _CustomTime.y * _Speed;
                
                // Add a time-based animation to the texture coordinate offset to make the texture move
                uv += float2(0, sin(_CustomTime.y * _WaveSpeed + IN.worldPos.x) * _WaveAmplitude);
            }
            else //Not paused
            {
                _CustomTime += _Time;
                // Calculate the texture coordinate offset based on flow direction and time
                uv += _FlowDirection * _CustomTime.y * _Speed;
                
                // Add a time-based animation to the texture coordinate offset to make the texture move
                uv += float2(0, sin(_CustomTime.y * _WaveSpeed + IN.worldPos.x) * _WaveAmplitude);
            }
            

            // Sample the texture at the offset coordinate and use the red channel as height
            float noise = tex2D(_MainTex, uv).r;

            // Calculate the height based on the sampled noise
            float height = noise * 0.1;

            // Set the surface output properties
            o.Albedo = _MainColour + height;
            o.Alpha = 1;

            // Apply normal mapping to create the illusion of waves
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.worldPos.xy * 0.01).rgba);
        }
        ENDCG
    }
    FallBack "Diffuse"
}