﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Expanse;

namespace Expanse
{
    public static class TransformExt
    {
        /// <summary>
        /// Destroys all game objects parented to a transform.
        /// </summary>
        public static void DestroyAllChildren(this Transform source, bool immediate = false)
        {
            for (int i = source.childCount - 1; i >= 0; i--)
            {
                if (immediate)
                    UnityEngine.Object.DestroyImmediate(source.GetChild(i).gameObject);
                else
                    UnityEngine.Object.Destroy(source.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Resets the position, rotation and scale to default values.
        /// </summary>
        public static void Reset(this Transform source)
        {
            source.localPosition = Vector3.zero;
            source.localRotation = Quaternion.identity;
            source.localScale = Vector3.one;
        }
    }
}
