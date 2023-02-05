using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Helper
{
    public class MathHelper
    {
        // TODO Helper function 
        /// <summary>
        /// Map a value from 
        /// </summary>
        /// <param name="oldMin"></param>
        /// <param name="oldMax"></param>
        /// <param name="newMin"></param>
        /// <param name="newMax"></param>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        public static float Map(float oldMin, float oldMax, float newMin, float newMax, float oldValue)
        {
            float OldRange = (oldMax - oldMin);
            float NewRange = (newMax - newMin);
            float NewValue = (((oldValue - oldMin) * NewRange) / OldRange) + newMin;

            return (NewValue);
        }
    }
}
