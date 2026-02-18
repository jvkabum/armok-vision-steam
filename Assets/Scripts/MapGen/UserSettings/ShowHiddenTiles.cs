using System;

namespace UserSettings
{
    public class ShowHiddenTiles : SliderBase
    {
        protected override void InitValue()
        {
            slider.value = Convert.ToInt32(GameSettings.Instance.rendering.showHiddenTiles);
            valueLabel.text = L.Get(GameSettings.Instance.rendering.showHiddenTiles.ToString());
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.rendering.showHiddenTiles = Convert.ToBoolean(value);
            valueLabel.text = L.Get(GameSettings.Instance.rendering.showHiddenTiles.ToString());
            gameMap.Refresh();
        }
    }
}
