﻿//-----------------------------------------------------------------------
// <copyright file="EnvironmentalLight.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore
{
    using UnityEngine;
    using UnityEngine.Rendering;

    /// <summary>
    /// A component that automatically adjust lighting settings for the scene
    /// to be inline with those estimated by ARCore.
    /// JPS: Added colorScale as a public variable.
    /// </summary>
    [ExecuteInEditMode]
    public class EnvironmentalLightEx : MonoBehaviour
    {
        public Texture testTex;

        [Range(0,1)]
        public float normalizedIntensity = 0.5f;

        [HideInInspector]
        public float colorScale;

        /// <summary>
        /// Unity update method that sets global light estimation shader constant to match
        /// ARCore's calculated values.
        /// </summary>
        public void Update()
        {
            // Use the following function to compute color scale:
            // * linear growth from (0.0, 0.0) to (1.0, LinearRampThreshold)
            // * slow growth from (1.0, LinearRampThreshold)
            const float linearRampThreshold = 0.8f;
            const float middleGray = 0.18f;
            const float inclination = 0.4f;

#if UNITY_ANDROID && !UNITY_EDITOR
            normalizedIntensity = Frame.LightEstimate.PixelIntensity / middleGray;
#endif

            float colorScale = 1.0f;

            if (normalizedIntensity < 1.0f)
            {
                colorScale = normalizedIntensity * linearRampThreshold;
            }
            else
            {
                float b = (linearRampThreshold / inclination) - 1.0f;
                float a = (b + 1.0f) / b * linearRampThreshold;
                colorScale = a * (1.0f - (1.0f / ((b * normalizedIntensity) + 1.0f)));
            }

            Shader.SetGlobalFloat("_GlobalLightEstimation", colorScale);
        }
    }
}

