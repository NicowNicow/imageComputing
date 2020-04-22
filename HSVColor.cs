using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace imageComputing {
    
    public class HSVColor {
        public float hue {get; private set;}
        public float saturation {get; private set;}
        public float value {get; private set;}

        public HSVColor(float h, float s, float v) {
            if (h < 0f) this.hue = 0f;
            else if (h > 360f) this.hue = 360f;
            else this.hue = h;

            if (s < 0f) this.saturation = 0f;
            else if (s > 1f) this.saturation = 1f;
            else this.saturation = s;

            if (v < 0f) this.value =  0f;
            else if (v > 1f) this.value =  1f;
            else this.value = v;
        }

        public static HSVColor RGBtoHSV(Color color) {
            float hue = 0f;
            float saturation = 0f;
            float[] rgbColors = new float[3]{color.R/255f, color.G/255f, color.B/255f};
            float minRGB = rgbColors.Min();
            float maxRGB = rgbColors.Max();
            float comparison = maxRGB - minRGB;
            if (comparison == 0) {
                hue=0;
                saturation=0;
            }
            else {
                saturation = (comparison)/maxRGB;
                float conversionR = (((comparison - rgbColors[0])/6f) + ((comparison)/2f))/(comparison);
                float conversionG = (((comparison - rgbColors[1])/6f) + ((comparison)/2f))/(comparison);
                float conversionB = (((comparison - rgbColors[2])/6f) + ((comparison)/2f))/(comparison);
                if (rgbColors[0] == maxRGB) hue = conversionB - conversionG;
                else if (rgbColors[1] == maxRGB) hue = (1f / 3f) + conversionR - conversionB;
                else if (rgbColors[2] == maxRGB) hue = (2f / 3f) + conversionG - conversionR;
                if (hue < 0f) hue = hue + 1;
                if (hue > 1f) hue = hue - 1;
            }
            return new HSVColor(hue*360f, saturation, maxRGB);
        }

        public static Color HSVtoRGB(HSVColor color) {
            if (color.saturation == 0) {
                int greyLevel = Convert.ToInt32(color.value*255);
                return Color.FromArgb(greyLevel, greyLevel, greyLevel);
            }
            else {
                float usableHue = color.hue/60f;
                if (usableHue == 6f) usableHue = 0f;
                float coeff1 = color.value * (1f - color.saturation);
                float coeff2 = color.value * (1f - color.saturation * (usableHue - (float)((int)usableHue)));
                float coeff3 = color.value * (1f - color.saturation* (1f - (usableHue - (int)usableHue)));
                switch((int)usableHue) {
                    case 0:
                        return Color.FromArgb((int)(color.value * 255f), (int)(coeff3 * 255f), (int)(coeff1 * 255f));
                    case 1:
                        return Color.FromArgb((int)(coeff2 * 255f), (int)(color.value * 255f), (int)(coeff1 * 255f));
                    case 2:
                        return Color.FromArgb((int)(coeff1 * 255f), (int)(color.value * 255f), (int)(coeff3 * 255f));
                    case 3:
                        return Color.FromArgb((int)(coeff1 * 255f), (int)(coeff2 * 255f), (int)(color.value * 255f));
                    case 4:
                        return Color.FromArgb((int)(coeff3 * 255f), (int)(coeff1 * 255f), (int)(color.value * 255f));
                    default:
                        return Color.FromArgb((int)(color.value * 255f), (int)(coeff1 * 255f), (int)(coeff2 * 255f));
                }
            }
        }

        public static Color HSVtoGreyLevelRGB(HSVColor color) {
            int greyLevel = Convert.ToInt32(color.value*255);
            return Color.FromArgb(greyLevel, greyLevel, greyLevel);
        }
    }
}