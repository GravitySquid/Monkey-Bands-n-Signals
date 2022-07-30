// -------------------------------------------------------------------------------------------------
//
//
// -------------------------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AutoRescale = false, AccessRights = AccessRights.None)]
    public class MonkeyBandsnSignals : Indicator
    {
        [Parameter("MA Periods", DefaultValue = 20)]
        public int Periods { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.WilderSmoothing)]
        public MovingAverageType MAType { get; set; }

        [Parameter("ATR Periods", DefaultValue = 110)]
        public int ATRPeriods { get; set; }

        [Parameter("ATR Band 1", DefaultValue = 2.6)]
        public double ATRB1Factor { get; set; }

        [Parameter("ATR Band 1", DefaultValue = 3.8)]
        public double ATRB2Factor { get; set; }

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

        AverageTrueRange ATR;
        MovingAverage MA;

        protected override void Initialize()
        {
            ATR = Indicators.AverageTrueRange(ATRPeriods, MovingAverageType.Simple);
            MA = Indicators.MovingAverage(Bars.MedianPrices, Periods, MAType);
        }

        public override void Calculate(int index)
        {

            Upper1[index] = MA.Result[index] + (ATRB1Factor * ATR.Result[index]);
            Upper2[index] = MA.Result[index] + (ATRB2Factor * ATR.Result[index]);
            Lower1[index] = MA.Result[index] - (ATRB1Factor * ATR.Result[index]);
            Lower2[index] = MA.Result[index] - (ATRB2Factor * ATR.Result[index]);
            Center[index] = MA.Result[index];
        }
    }
}
