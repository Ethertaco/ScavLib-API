using UnityEngine;

namespace ScavLib.gui.ugui
{
    public class UguiPixelBar : MonoBehaviour
    {
        internal UnityEngine.UI.Image image;
        private float _fill;

        public float Fill
        {
            get => _fill;
            set { _fill = Mathf.Clamp01(value); Apply(); }
        }

        private void Apply()
        {
            if (image == null) return;
            if (image.sprite == null)
            {
                image.fillAmount = _fill;
                return;
            }
            float texSize = (image.fillMethod == UnityEngine.UI.Image.FillMethod.Vertical)
                ? image.sprite.texture.height
                : image.sprite.texture.width;
            image.fillAmount = Mathf.Round(_fill * texSize) / texSize;
        }

        private void Update() => Apply();
    }
}
