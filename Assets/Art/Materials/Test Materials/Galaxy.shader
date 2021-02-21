Shader "Unlit/Galaxy"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			// Created by sebastien durand - 08/2016
//-------------------------------------------------------------------------------------
// Based on "Dusty nebula 4" by Duke (https://www.shadertoy.com/view/MsVXWW) 
// Sliders from IcePrimitives by Bers (https://www.shadertoy.com/view/MscXzn)
// License: Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License
//-------------------------------------------------------------------------------------


#define R(p, a) p=cos(a)*p+sin(a)*float2(p.y, -p.x)
#define pi 3.14159265


			const float4
				colCenter = float4(1.2, 1.5, 1.5, .25),
				colEdge = float4(.1, .1, .2, .5),
				colEdge2 = float4(.7, .54, .3, .23),
				colEdge3 = float4(.6, 1., 1.3, .25);

			const float time = 10.;
			float4 sliderVal;

			float2 min2(float2 a, float2 b) {
				return a.x < b.x ? a : b;
			}

			float hash(const in float3 p) {
				float h = dot(p, float3(127.1, 311.7, 758.5453123));
				return frac(sin(h) * 43758.5453123);
			}

			// [iq] https://www.shadertoy.com/view/4sfGzS
			float noiseText(in float3 x) {
				float3 p = floor(x), f = frac(x);
				f = f * f * (3. - f - f);
				float2 uv = (p.xy + float2(37., 17.) * p.z) + f.xy,
					rg = textureLod(iChannel0, (uv + .5) / 256., -100.).yx;
				return lerp(rg.x, rg.y, f.z);
			}

			// ratio: ratio of hight/low frequencies
			float fbmdust(in float3 p, in float ratio) {
				return lerp(noiseText(p * 3.), noiseText(p * 20.), ratio);
			}

			float2 spiralArm(in float3 p, in float thickness, in float blurAmout, in float blurStyle) {
				float dephase = 2.2, loop = 4.;
				float a = atan2(p.x, p.z),  // angle     
					r = length(p.xz), lr = log(r), // distance to center
					th = (.1 - .25 * r), // thickness according to distance
					d = frac(.5 * (a - lr * loop) / pi); //apply rotation and scaling.
				d = (.5 / dephase - abs(d - .5)) * 2. * pi * r;
				d *= (1. - lr) / thickness;  // space fct of distance
				// Perturb distance field
				float radialBlur = blurAmout * fbmdust(float3(r * 4., 10. * d, 10. - 5. * p.y), blurStyle);
				return float2(sqrt(d * d + 10. * p.y * p.y / thickness) - th * r * .2 - radialBlur);
			}

			float2 dfGalaxy(in float3 p, in float thickness, in float blurAmout, in float blurStyle) {
				return min2(spiralArm(p, thickness, blurAmout, blurStyle),
					spiralArm(float3(p.z, p.y, -p.x), thickness, blurAmout, blurStyle));
			}

			float2 map(in float3 p) {
				R(p.xz, iMouse.x * .008 * pi + _Time.y * .3);
				return dfGalaxy(p, clamp(10. * sliderVal.x, .9, 10.), sliderVal.y, sliderVal.z);
			}

			//--------------------------------------------------------------

			// assign color to the media
			float4 computeColor(in float3 p, in float density, in float radius, in float id) {
				// color based on density alone, gives impression of occlusion within
				// the media
				float4 result = lerp(float4(1., .9, .8, 1.), float4(.4, .15, .1, 1.), density);
				// color added to the media
				result *= lerp(colCenter,
					lerp(colEdge2,
						lerp(colEdge, colEdge3, step(.08, id)), step(-.05, id)),
					smoothstep(.2, .8, radius));
				return result;
			}

			// - Ray / Shapes Intersection -----------------------
			bool sBox(in float3 ro, in float3 rd, in float3 rad, out float tN, out float tF) {
				float3 m = 1. / rd, n = m * ro,
					k = abs(m) * rad,
					t1 = -n - k, t2 = -n + k;
				tN = max(max(t1.x, t1.y), t1.z);
				tF = min(min(t2.x, t2.y), t2.z);
				return !(tN > tF || tF < 0.);
			}

			bool sSphere(in float3 ro, in float3 rd, in float r, out float tN, out float tF) {
				float b = dot(rd, ro), d = b * b - dot(ro, ro) + r;
				if (d < 0.) return false;
				tN = -b - sqrt(d);
				tF = -tN - b - b;
				return tF > 0.;
			}

			// ---------------------------------------------------
			// Bers : https://www.shadertoy.com/view/MscXzn
			float4 processSliders(in float2 uv) {
				sliderVal = tex2D(iChannel2, float2(0));
				if (length(uv.xy) > 1.) {
					return tex2D(iChannel2, uv.xy / iResolution.xy);
				}
				return float4(0);
			}

			// ---------------------------------------------------
			// Based on "Dusty nebula 4" by Duke (https://www.shadertoy.com/view/MsVXWW) 
			void mainImage(out float4 fragColor, in float2 fragCoord) {
				float4 cSlider = processSliders(fragCoord);

				// camera	   
				float a = sliderVal.w * pi;
				float3 ro = float3(0., 2. * cos(a), -4.5 * sin(a)),
					ta = float3(-.2, -.3, 0);

				// camera tx
				float3 cw = normalize(ta - ro),
					cp = float3(0., 1., 0.),
					cu = normalize(cross(cw, cp)),
					cv = normalize(cross(cu, cw));
				float2 q = (fragCoord.xy) / iResolution.xy,
					p = -1. + 2. * q;
				p.x *= iResolution.x / iResolution.y;

				float3 rd = normalize(p.x * cu + p.y * cv + 2.5 * cw);

				// ld, td: local, total density 
				// w: weighting factor
				float ld = 0., td = 0., w = 0.;

				// t: length of the ray
				// d: distance function
				float d = 1., t = 0.;

				const float h = 0.1;

				float4 sum = float4(0);

				float min_dist = 0., max_dist = 0.,
					min_dist2 = 0., max_dist2 = 0.;

				if (sSphere(ro, rd, 4., min_dist, max_dist)) {
					if (sBox(ro, rd, float3(4., 1.8, 4.), min_dist2, max_dist2)) {
						min_dist = max(.1, max(min_dist, min_dist2));
						max_dist = min(max_dist, max_dist2);

						t = min_dist * step(t, min_dist) + .1 * hash(rd + _Time.y);


						// raymarch loop
						float4 col;
						for (int i = 0; i < 100; i++) {
							float3 pos = ro + t * rd;

							// Loop break conditions.
							if (td > .9 || sum.a > .99 || t > max_dist) break;

							// evaluate distance function
							float2 res = map(pos);
							d = max(res.x, .01);

							// point light calculations
							float3 ldst = pos;
							ldst.y *= 1.6;
							float3 ldst2 = pos;
							ldst2.y *= 3.6;
							float lDist = max(length(ldst), .1), //max(length(ldst), 0.001);
								lDist2 = max(length(ldst2), .1);
							// star in center
							float3 lightColor = (1. - smoothstep(3., 4.5, lDist * lDist)) *
								lerp(.015 * float3(1., .5, .25) / (lDist * lDist),
									.02 * float3(.5, .7, 1.) / (lDist2 * lDist2),
									smoothstep(.1, 2., lDist * lDist));
							sum.rgb += lightColor; //.015*lightColor/(lDist*lDist); // star itself and bloom around the light
							sum.a += .003 / (lDist * lDist);;

							if (d < h) {
								// compute local density 
								ld = h - d;
								// compute weighting factor 
								w = (1. - td) * ld;
								// accumulate density
								td += w + 1. / 60.;
								// get color of object (with transparencies)
								col = computeColor(pos, td, lDist * 2., res.y);
								col.a *= td;
								// colour by alpha
								col.rgb *= col.a;
								// alpha blend in contribution
								sum += col * (1.0 - sum.a);
							}

							//float pitch = t/iResolution.x;
							//float dt = max(d * 0.25, .005); //pitch);
							// trying to optimize step size near the camera and near the light source
							t += max(d * .15 * max(min(length(ldst), length(ro)), 1.0), 0.005);
							td += .1 / 70.;
							//t += dt;
						}
						// simple scattering
						sum *= 1. / exp(ld * .2) * .8;
						sum = clamp(sum, 0., 1.);
					}
				}

				// Background color
				sum.rgb += float3(clamp(2. * cos(.5 * _Time.y), 0., .4)) * (1. - sum.a) * pow(16.0 * q.x * q.y * (1. - q.x) * (1. - q.y), .3);

				//Apply slider overlay
				fragColor = float4(lerp(sum.xyz, cSlider.rgb, cSlider.a), 1.);

			}

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
