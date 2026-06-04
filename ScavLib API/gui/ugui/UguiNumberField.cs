using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace ScavLib.gui.ugui
{

    public sealed class UguiNumberField : MonoBehaviour
    {
        private TMP_InputField _input;
        private float _value;
        private float _step;
        private float _min;
        private float _max;
        private bool _isInteger;
        private Action<float> _onChange;
        private bool _suppress;

        public float Value => _value;

        internal void Init(TMP_InputField input, float initial, float step,
            float min, float max, bool isInteger, Action<float> onChange)
        {
            _input = input;
            _step = step;
            _min = min;
            _max = max;
            _isInteger = isInteger;
            _onChange = onChange;

            _input.contentType = isInteger
                ? TMP_InputField.ContentType.IntegerNumber
                : TMP_InputField.ContentType.DecimalNumber;

            _input.onEndEdit.AddListener(OnInputEndEdit);

            SetValue(initial, notify: false);
        }

        public void Step(int direction)
        {
            SetValue(_value + _step * direction, notify: true);
        }

        public void SetValue(float v, bool notify)
        {
            float clamped = Mathf.Clamp(v, _min, _max);
            if (_isInteger) clamped = Mathf.Round(clamped);
            _value = clamped;

            if (_input != null)
            {
                _suppress = true;
                _input.text = FormatValue(_value);
                _suppress = false;
            }

            if (notify) _onChange?.Invoke(_value);
        }

        private void OnInputEndEdit(string text)
        {
            if (_suppress) return;
            if (TryParse(text, out float parsed))
                SetValue(parsed, notify: true);
            else
                SetValue(_value, notify: false);
        }

        private bool TryParse(string text, out float result)
        {
            return float.TryParse(text, NumberStyles.Float | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture, out result);
        }

        private string FormatValue(float v)
        {
            return _isInteger
                ? ((int)v).ToString(CultureInfo.InvariantCulture)
                : v.ToString("0.###", CultureInfo.InvariantCulture);
        }
    }
}
