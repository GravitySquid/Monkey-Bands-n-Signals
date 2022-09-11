// -------------------------------------------------------------------------------------------------
//
//
// -------------------------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;
using System.Collections.Generic;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AutoRescale = false, AccessRights = AccessRights.None)]
    public class MonkeyBandsnSignals : Indicator
    {

        [Parameter("MA Periods", DefaultValue = 20, Group = "Bands")]
        public int MAPeriods { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.WilderSmoothing, Group = "Bands")]
        public MovingAverageType MAType { get; set; }

        [Parameter("ATR Periods", DefaultValue = 110, Group = "Bands")]
        public int ATRPeriods { get; set; }

        [Parameter("ATR Band 1", DefaultValue = 2.6, Group = "Bands")]
        public double ATRB1Factor { get; set; }

        [Parameter("ATR Band 1", DefaultValue = 3.8, Group = "Bands")]
        public double ATRB2Factor { get; set; }

        [Parameter("Show Fractals", DefaultValue = true, Group = "Fractals")]
        public bool ShowFractals { get; set; }

        [Parameter("Fractal periods", DefaultValue = 5, Step = 1, MinValue = 1, Group = "Fractals")]
        public int FractalPeriods { get; set; }

        [Parameter("Trend MA Periods", DefaultValue = 500, Group = "Trend Detection")]
        public int TrendMAPeriods { get; set; }

        [Parameter("Trend MA Type", DefaultValue = MovingAverageType.Exponential, Group = "Trend Detection")]
        public MovingAverageType TrendMAType { get; set; }

        [Output("Upper 1", LineColor = "RoyalBlue")]
        public IndicatorDataSeries Upper1 { get; set; }

        [Output("Upper 2", LineColor = "BlueViolet")]
        public IndicatorDataSeries Upper2 { get; set; }

        [Output("Lower 1", LineColor = "RoyalBlue")]
        public IndicatorDataSeries Lower1 { get; set; }

        [Output("Lower 2", LineColor = "BlueViolet")]
        public IndicatorDataSeries Lower2 { get; set; }

        [Output("Center", LineColor = "Violet")]
        public IndicatorDataSeries Center { get; set; }

        private const String arrowUp = "▲";
        private const String arrowDown = "▼";
        private const String circle = "◯";

        private AverageTrueRange ATR;
        private MovingAverage MA, TrendMA;
        private StochasticOscillator Stoch;

        protected override void Initialize()
        {
            //System.Diagnostics.Debugger.Launch();
            ATR = Indicators.AverageTrueRange(ATRPeriods, MovingAverageType.Simple);
            MA = Indicators.MovingAverage(Bars.MedianPrices, MAPeriods, MAType);
            TrendMA = Indicators.MovingAverage(Bars.MedianPrices, TrendMAPeriods, TrendMAType);
            Stoch = Indicators.StochasticOscillator(8, 3, 5, MovingAverageType.Simple);

        }

        public override void Calculate(int index)
        {
            // Bands
            Upper1[index] = MA.Result[index] + (ATRB1Factor * ATR.Result[index]);
            Upper2[index] = MA.Result[index] + (ATRB2Factor * ATR.Result[index]);
            Lower1[index] = MA.Result[index] - (ATRB1Factor * ATR.Result[index]);
            Lower2[index] = MA.Result[index] - (ATRB2Factor * ATR.Result[index]);
            Center[index] = MA.Result[index];

            // Mark Swing H/L
            if (index < FractalPeriods)
                return;

            DrawUpFractal(index);
            DrawDownFractal(index);

        }

        private void DrawUpFractal(int index)
        {
            int period = FractalPeriods % 2 == 0 ? FractalPeriods - 1 : FractalPeriods;
            int middleIndex = index - period / 2;
            double middleValue = Bars.HighPrices[middleIndex];

            bool up = true;

            for (int i = 0; i < period; i++)
            {
                if (middleValue < Bars.HighPrices[index - i])
                {
                    up = false;
                    break;
                }
            }
            if (up)
            {
                var x = Chart.DrawText("FractalUp_" + index, circle, middleIndex, Bars.HighPrices[middleIndex], Color.Aqua);
                x.VerticalAlignment = VerticalAlignment.Center;
                x.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }

        private void DrawDownFractal(int index)
        {
            int period = FractalPeriods % 2 == 0 ? FractalPeriods - 1 : FractalPeriods;
            int middleIndex = index - period / 2;
            double middleValue = Bars.LowPrices[middleIndex];
            bool down = true;

            for (int i = 0; i < period; i++)
            {
                if (middleValue > Bars.LowPrices[index - i])
                {
                    down = false;
                    break;
                }
            }
            if (down)
            {
                var x = Chart.DrawText("FractalDown_" + index, circle, middleIndex, Bars.LowPrices[middleIndex], Color.Aqua);
                x.VerticalAlignment = VerticalAlignment.Center;
                x.HorizontalAlignment = HorizontalAlignment.Center;

            }

        }

    }
}
