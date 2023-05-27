using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Rendering.PostProcessing
{
    [System.Serializable]
    public sealed class IntParameter_NoInterp : ParameterOverride<int>
    {
        public override void Interp(int from, int to, float t){
            value = t>0?to:from;
        }
    }
}
