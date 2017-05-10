﻿using UnityEngine;
using UnityEngine.UI;

namespace Expanse
{
    /// <summary>
    /// Motion that moves an Graphic color value towards a target color.
    /// </summary>
    public class GraphicColorMotion : ColorMotion
    {
        public Graphic Graphic { get; private set; }

        public GraphicColorMotion() : base(1, null, null) { }
        public GraphicColorMotion(float duration) : base(duration, null, null) { }
        public GraphicColorMotion(float duration, CallBackRelay cbr) : base(duration, cbr, null) { }
        public GraphicColorMotion(float duration, MonoBehaviour attachedMonobehaviour) : base(duration, null, attachedMonobehaviour) { }
        public GraphicColorMotion(float duration, CallBackRelay cbr, MonoBehaviour attachedMonobehaviour) : base(duration, cbr, attachedMonobehaviour) { }

        public void SetParameters(Graphic graphic, Color targetColor)
        {
            Graphic = graphic;
            SetParameters(() => this.Graphic.color, targetColor);
        }

        public void SetParameters(Graphic graphic, Color startColor, Color targetColor)
        {
            Graphic = graphic;
            SetParameters(startColor, targetColor);
        }

        protected override void OnValueChanged()
        {
            if (Graphic != null)
            {
                Graphic.color = CurrentValue;
            }
        }
    }
}