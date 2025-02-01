//
// Copyright (C) 2024, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//
#region Using declarations
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators
{
	public class HeadAndShoulders : Indicator
	{
		private NinjaTrader.Gui.Tools.SimpleFont myFont;
		private Swing Swing1;
		private Swing Swing2;

		
		private int predictiveHighBarsAgo;
		private int predictiveLowBarsAgo;
		private double predictiveSwingHigh;
		private double predictiveSwingLow;
		
		private int swingHighBarsAgo;
		private int swingLowBarsAgo;
		private double latestSwingHigh;
		private double latestSwingLow;
		private int latestSwingHighBar;
		private int latestSwingLowBar;
		private int cacheLatestSwingHighBar;
		private int cacheLatestSwingLowBar;
		
		private int previousSwingHighBarsAgo;
		private int previousSwingLowBarsAgo;
		private double previousSwingHigh;
		private double previousSwingLow;
		

		
		private int previousPredictiveLowBarsAgo;
		private int previousPredictiveHighBarsAgo;
		private int latestPredictiveSwingHighBar;
		private int latestPredictiveSwingLowBar;
		private int previousPredictiveSwingHighBar;
		private int previousPredictiveSwingLowBar;
		
		private int precedingSwingHighBarsAgo;
		private int precedingSwingLowBarsAgo;
		private double precedingSwingHigh;
		private double precedingSwingLow;
		
		private bool bearStep1Complete;
		private bool bearStep2Complete;
		private bool bearStep3Complete;
		private bool bearStep4Complete;
		private bool bearStep5Complete;
		
		private bool bullStep1Complete;
		private bool bullStep2Complete;
		private bool bullStep3Complete;
		private bool bullStep4Complete;
		private bool bullStep5Complete;
		
		private double bearFibLevel1;
		private double bullFibLevel1;
		
		private double minimumBearShoulderOne;
		private double minimumBullShoulderOne;
		
		private Text latestBearText;
		private Line latestBearLine;
		
		private Text latestBullText;
		private Line latestBullLine;
		
		private int cacheLatestHeadAndShoulderBullBar;
		private int cacheLatestHeadAndShoulderBearBar;
		
		
		
		

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Calculate					= Calculate.OnBarClose;
				Description					= "HeadAndShoulders by Alighten";
				Name						= "HeadAndShoulders";
				DrawOnPricePanel			= true;
				IsOverlay					= true;
				IsSuspendedWhileInactive	= true;

				SwingStrength = 10;
				PredictiveSwingStrength = 3;
				MinimumShoulderTwoRetracementPercent = 30.0;
				MinimumShoulderOneRetracementPercent = 50.0;
				HeadAndShoulderShortMessage = "Get Ready Short";
				HeadAndShoulderLongMessage = "Get Ready Long";
				
				myFont = new NinjaTrader.Gui.Tools.SimpleFont("Arial", 12) { Size = 25, Bold = true };
				cacheLatestSwingHighBar = 0;
				cacheLatestSwingLowBar = 0;
				
				AddPlot(new Stroke(Brushes.Aqua, 6), PlotStyle.TriangleUp, "TmaLongTriangle");
            	AddPlot(new Stroke(Brushes.Red, 6), PlotStyle.TriangleDown, "TmaShortTriangle");
				
			
			}
			else if (State == State.Historical)
			{
				
			}
			else if (State == State.DataLoaded)
			{
				ClearOutputWindow();
				
				Swing1 = Swing(SwingStrength);
				Swing2 = Swing(PredictiveSwingStrength);
								
				
			}
		}

		private void ResetBearSteps()
		{
			
			bearStep4Complete = false;
		}
		private void ResetBullSteps()
		{
			
			bullStep4Complete = false;
		}
		
		protected override void OnBarUpdate()
		{
			if (CurrentBar < SwingStrength)
                return;
			
			
			//Value[0] = 0;
			bearStep1Complete = false;
			bearStep2Complete = false;
			bearStep3Complete = false;
			bearStep5Complete = false;
			
			bullStep1Complete = false;
			bullStep2Complete = false;
			bullStep3Complete = false;
			bullStep5Complete = false;
			
				
			//Use these swings to temporarily calculate the Fibonacci Retracement
			predictiveHighBarsAgo = Swing2.SwingHighBar(0, 1, Bars.BarsSinceNewTradingDay);
			predictiveLowBarsAgo = Swing2.SwingLowBar(0, 1, Bars.BarsSinceNewTradingDay);
			latestPredictiveSwingHighBar = CurrentBar - predictiveHighBarsAgo;
			latestPredictiveSwingLowBar = CurrentBar - predictiveLowBarsAgo;
			
			//Use these swings to calculate the Fibonacci Retracement
			swingHighBarsAgo = Swing1.SwingHighBar(0, 1, Bars.BarsSinceNewTradingDay);
			swingLowBarsAgo = Swing1.SwingLowBar(0, 1, Bars.BarsSinceNewTradingDay);
			latestSwingHighBar = CurrentBar - swingHighBarsAgo;
			latestSwingLowBar = CurrentBar - swingLowBarsAgo;
			
			previousSwingHighBarsAgo = Swing1.SwingHighBar(0, 2, Bars.BarsSinceNewTradingDay);
			previousSwingLowBarsAgo = Swing1.SwingLowBar(0, 2, Bars.BarsSinceNewTradingDay);
			
			precedingSwingHighBarsAgo = Swing1.SwingHighBar(0, 3, Bars.BarsSinceNewTradingDay);
			precedingSwingLowBarsAgo = Swing1.SwingLowBar(0, 3, Bars.BarsSinceNewTradingDay);
			
			
			if(previousSwingHighBarsAgo > -1 && previousSwingLowBarsAgo > -1)
			{
				
				try
				{			
					
					predictiveSwingHigh = Swing2.SwingHigh[predictiveHighBarsAgo];
					predictiveSwingLow =  Swing2.SwingLow[predictiveLowBarsAgo];
					
					latestSwingHigh = Swing1.SwingHigh[swingHighBarsAgo];
					latestSwingLow =  Swing1.SwingLow[swingLowBarsAgo];
					
					previousSwingHigh = Swing1.SwingHigh[previousSwingHighBarsAgo];
					previousSwingLow = Swing1.SwingLow[previousSwingLowBarsAgo];
					
					precedingSwingHigh = Swing1.SwingHigh[precedingSwingHighBarsAgo];
					precedingSwingLow = Swing1.SwingLow[precedingSwingLowBarsAgo];
					
				}
				catch (Exception e)
				{
					// In case the indicator has already been Terminated, you can safely ignore errors
					if (State >= State.Terminated)
						return;
					
					Log("HeadAndShoulders", NinjaTrader.Cbi.LogLevel.Warning);
					
					Print(Time[0] + " " + e.ToString());
				}
				
				
				////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				/// BEAR SCENARIO
				///////////////////////////////////////////////////////////////////////////////////////////////////////////////
				bearFibLevel1 = ((High[swingHighBarsAgo] - Low[predictiveLowBarsAgo]) * MinimumShoulderTwoRetracementPercent/100) + Low[predictiveLowBarsAgo];
				
				minimumBearShoulderOne = ((High[swingHighBarsAgo] - Low[previousSwingLowBarsAgo]) * MinimumShoulderOneRetracementPercent/100) + Low[previousSwingLowBarsAgo];
				if (latestPredictiveSwingLowBar == latestSwingLowBar)
				{
					minimumBearShoulderOne = ((High[swingHighBarsAgo] - Low[swingLowBarsAgo]) * MinimumShoulderOneRetracementPercent/100) + Low[swingLowBarsAgo];
				}
				
				if (CrossBelow(Close, predictiveSwingLow, 1))
					ResetBearSteps();				

				if (previousSwingHigh < latestSwingHigh)
					bearStep2Complete = true;
				if (latestSwingLow > predictiveSwingLow)
					bearStep3Complete = true;
				if (latestPredictiveSwingLowBar == latestSwingLowBar)
				{
					if (previousSwingLow > latestSwingLow)
						bearStep3Complete = true;
				}
				if (CrossAbove(Close, bearFibLevel1, 4))
					bearStep4Complete = true;
				if (previousSwingHigh > minimumBearShoulderOne)
					bearStep5Complete = true;
				

				if (bearStep2Complete && bearStep3Complete && bearStep4Complete && bearStep5Complete && latestSwingHighBar != cacheLatestSwingHighBar)  //bearStep1Complete
				{
					latestBearText = Draw.Text(this, "H&S_Short_Text_" + CurrentBar, false, "Get Ready Short", swingHighBarsAgo, High[swingHighBarsAgo], 25, Brushes.White, myFont, TextAlignment.Left, Brushes.Red, Brushes.Red, 100);
					latestBearLine = Draw.Line(this, "H&S_Short_Line_" + CurrentBar, false, 0, High[0], 0, High[swingHighBarsAgo], Brushes.Red, DashStyleHelper.Solid, 2);
					DrawOnPricePanel = true;
					cacheLatestSwingHighBar = latestSwingHighBar;
					cacheLatestHeadAndShoulderBearBar = CurrentBar;
					ResetBearSteps();
				}
				
				if (cacheLatestSwingHighBar == latestSwingHighBar && CrossAbove(High, High[swingHighBarsAgo], 1))
				{
					RemoveDrawObject("H&S_Short_Text_" + cacheLatestHeadAndShoulderBearBar);
					RemoveDrawObject("H&S_Short_Line_" + cacheLatestHeadAndShoulderBearBar);
					latestBearText = null;
					latestBearLine = null;
				}
				
				
				
				
				////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				/// BULL SCENARIO
				///////////////////////////////////////////////////////////////////////////////////////////////////////////////
				bullFibLevel1 = High[predictiveHighBarsAgo] - ((High[predictiveHighBarsAgo] - Low[swingLowBarsAgo]) * MinimumShoulderTwoRetracementPercent/100);
				
				
				minimumBullShoulderOne = High[swingHighBarsAgo] - ((High[swingHighBarsAgo] - Low[swingLowBarsAgo]) * MinimumShoulderOneRetracementPercent/100);
				if (latestPredictiveSwingLowBar == latestSwingLowBar)
				{
					minimumBullShoulderOne = High[previousSwingHighBarsAgo] - ((High[previousSwingHighBarsAgo] - Low[swingLowBarsAgo]) * MinimumShoulderOneRetracementPercent/100);
				}
				
				if (CrossAbove(Close, predictiveSwingHigh, 1))
					ResetBullSteps();				

				if (previousSwingLow > latestSwingLow)
					bullStep2Complete = true;
				if (latestSwingHigh < predictiveSwingHigh)
					bullStep3Complete = true;
				if (latestPredictiveSwingHighBar == latestSwingHighBar)
				{
					if (previousSwingHigh < latestSwingHigh)
						bullStep3Complete = true;
				}
				if (CrossBelow(Close, bullFibLevel1, 4))
					bullStep4Complete = true;
				if (previousSwingLow < minimumBullShoulderOne)
					bullStep5Complete = true;
				
				if (bullStep2Complete && bullStep3Complete && bullStep4Complete && bullStep5Complete && latestSwingLowBar != cacheLatestSwingLowBar) //bullStep1Complete
				{
					latestBullText = Draw.Text(this, "H&S_Long_Text_" + CurrentBar, false, "Get Ready Long", swingLowBarsAgo, Low[swingLowBarsAgo], -25, Brushes.White, myFont, TextAlignment.Left, Brushes.Green, Brushes.Green, 100);
					latestBullLine = Draw.Line(this, "H&S_Long_Line_" + CurrentBar, false, 0, Low[0], 0, Low[swingLowBarsAgo], Brushes.Green, DashStyleHelper.Solid, 2);
					DrawOnPricePanel = true;
					cacheLatestSwingLowBar = latestSwingLowBar;
					cacheLatestHeadAndShoulderBullBar = CurrentBar;
					ResetBullSteps();
				}
				if (cacheLatestSwingLowBar == latestSwingLowBar && CrossBelow(Low, Low[swingLowBarsAgo], 1))
				{
					RemoveDrawObject("H&S_Long_Text_" + cacheLatestHeadAndShoulderBullBar);
					RemoveDrawObject("H&S_Long_Line_" + cacheLatestHeadAndShoulderBullBar);
					latestBullText = null;
					latestBullLine = null;
				}
			}

		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="SwingStrength", Order=1, GroupName="Parameters")]
		public int SwingStrength
		{ get; set; }
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="PredictiveSwingStrength", Order=2, GroupName="Parameters")]
		public int PredictiveSwingStrength
		{ get; set; }
		[Display(Name="MinimumShoulderOneRetracementPercent", Order=3, GroupName="Parameters")]
		public double MinimumShoulderOneRetracementPercent
		{ get; set; }	
		[Display(Name="MinimumShoulderTwoRetracementPercent", Order=4, GroupName="Parameters")]
		public double MinimumShoulderTwoRetracementPercent
		{ get; set; }		
		[Display(Name="HeadAndShoulderShortMessage", Order=5, GroupName="Parameters")]
		public string HeadAndShoulderShortMessage
		{ get; set; }		
		[Display(Name="HeadAndShoulderLongMessage", Order=6, GroupName="Parameters")]
		public string HeadAndShoulderLongMessage
		{ get; set; }
		
		
		
		
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private HeadAndShoulders[] cacheHeadAndShoulders;
		public HeadAndShoulders HeadAndShoulders(int swingStrength, int predictiveSwingStrength)
		{
			return HeadAndShoulders(Input, swingStrength, predictiveSwingStrength);
		}

		public HeadAndShoulders HeadAndShoulders(ISeries<double> input, int swingStrength, int predictiveSwingStrength)
		{
			if (cacheHeadAndShoulders != null)
				for (int idx = 0; idx < cacheHeadAndShoulders.Length; idx++)
					if (cacheHeadAndShoulders[idx] != null && cacheHeadAndShoulders[idx].SwingStrength == swingStrength && cacheHeadAndShoulders[idx].PredictiveSwingStrength == predictiveSwingStrength && cacheHeadAndShoulders[idx].EqualsInput(input))
						return cacheHeadAndShoulders[idx];
			return CacheIndicator<HeadAndShoulders>(new HeadAndShoulders(){ SwingStrength = swingStrength, PredictiveSwingStrength = predictiveSwingStrength }, input, ref cacheHeadAndShoulders);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.HeadAndShoulders HeadAndShoulders(int swingStrength, int predictiveSwingStrength)
		{
			return indicator.HeadAndShoulders(Input, swingStrength, predictiveSwingStrength);
		}

		public Indicators.HeadAndShoulders HeadAndShoulders(ISeries<double> input , int swingStrength, int predictiveSwingStrength)
		{
			return indicator.HeadAndShoulders(input, swingStrength, predictiveSwingStrength);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.HeadAndShoulders HeadAndShoulders(int swingStrength, int predictiveSwingStrength)
		{
			return indicator.HeadAndShoulders(Input, swingStrength, predictiveSwingStrength);
		}

		public Indicators.HeadAndShoulders HeadAndShoulders(ISeries<double> input , int swingStrength, int predictiveSwingStrength)
		{
			return indicator.HeadAndShoulders(input, swingStrength, predictiveSwingStrength);
		}
	}
}

#endregion
