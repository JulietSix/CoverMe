/* 
 * CoverMe! - Copyright (C) 2015 Juliet_Six
 * 
 * This file is part of "CoverMe!". "CoverMe!" is licensed under
 * the Microsoft Public License. You should have received a copy
 * of this license together with this file. Otherwise, see 
 * http://www.microsoft.com/en-us/openness/licenses.aspx
 *
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using WindowsInput;
using WT_Logger;

namespace CoverMe
{
	/// <summary>
	/// The very basic form shown when CoverMe is running
	/// </summary>
	public partial class MainForm : Form
	{
		public const int TRIGGER_TIMEOUT = 1000;
		public const int MAXIMUM_TRIES = 3;
		
		Stopwatch smartTriggerTimeoutWatch;
		Stopwatch coverMeTimeoutWatch;
		
		//int lastSmartTriggerStageTime;
		//int lastTriggerTime;
		int smartTriggerStage;
		uint errorCode;

		List<VirtualKeyCode> simpleTriggerCodes;
		
		List<VirtualKeyCode> quickChatCodes;
		
		int smartTriggerTimeout = 1000;
		bool useSmartTrigger = true;
		
		bool typeHeading = true;
		bool lowFPSmode = false;
		
		public MainForm()
		{
			InitializeComponent();
		}
		
		void MainFormShown(object sender, EventArgs e)
		{
			try {
				// Load settings
				Properties.LoadSettings();

				simpleTriggerCodes = Helpers.ParseEnumArray<VirtualKeyCode>(Properties.GetSetting("SimpleTriggerKeys", "VK_Z"));
				Logger.log("Loaded setting SimpleTriggerKeysr=" + Properties.GetSetting("SimpleTriggerKeys", "VK_Z"));
				
				useSmartTrigger = Boolean.Parse(Properties.GetSetting("UseSmartTrigger", "True"));
				Logger.log("Loaded setting UseSmartTrigger=" + useSmartTrigger.ToString());
				
				quickChatCodes = Helpers.ParseEnumArray<VirtualKeyCode>(Properties.GetSetting("QuickChatKeys", "VK_K,VK_T"));
				Logger.log("Loaded setting QuickChatKeys=" + Properties.GetSetting("QuickChatKeys", "VK_K,VK_T"));
				
				smartTriggerTimeout = Int32.Parse(Properties.GetSetting("SmartTriggerTimeout", "1000"));
				Logger.log("Loaded setting SmartTriggerTimeout=" + smartTriggerTimeout.ToString());
				
				typeHeading = Boolean.Parse(Properties.GetSetting("TypeHeading", "True"));
				Logger.log("Loaded setting TypeHeading=" + typeHeading.ToString());
				
				lowFPSmode = Boolean.Parse(Properties.GetSetting("LowFpsMode", "False"));
				Logger.log("Loaded setting LowFpsMode=" + lowFPSmode.ToString());
				
				SpecialKeyHelper.StrokeDelay = lowFPSmode ? 200 : 50;

				smartTriggerTimeoutWatch = Stopwatch.StartNew();
				smartTriggerStage = 0;
				
				coverMeTimeoutWatch = Stopwatch.StartNew();
				
				// Prepare UI elements
				if (useSmartTrigger)
					labelInfo.Text = "Smart trigger is enabled";
				else
					labelInfo.Text = "Press " + simpleTriggerCodes.ToString() + " to type altitude";
				if (typeHeading)
					labelInfo.Text += Environment.NewLine + "HDG is enabled";
				
				timerUpdate.Start();
				
				Logger.log("Initialisation complete, starting timer");
			}
			catch (Exception ex)
			{
				Logger.logException("Error initialising form", ex);
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Environment.Exit(1);
			}
		}
		
		void TimerUpdateTick(object sender, EventArgs e)
		{
			if (isTriggered() && canRetriggerAlready())
			{
				Logger.log("Firing CoverMe-action");
				
				if (!backgroundWorker.IsBusy)
					backgroundWorker.RunWorkerAsync();
				else
					Logger.log("Worker is busy. Aborting");
			}
		}
		
		/// <summary>
		/// Checks wether CoverMe! can already post the altitude again, or if
		/// the timeout criteria is not met yet. This prevents repeatedly re-
		/// triggering if keys are held down.
		/// </summary>
		/// <returns>True if can retrigger, otherwise false</returns>
		bool canRetriggerAlready() {
			if (coverMeTimeoutWatch.ElapsedMilliseconds > TRIGGER_TIMEOUT)
			{
				Helpers.RestartStopwatch(coverMeTimeoutWatch);
				return true;
			}
			else
			{
				Logger.log("Cannot retrigger already, timeout not met");
				return false;
			}
		}
		
		/// <summary>
		/// Checks wether the current Trigger criteria is met
		/// </summary>
		/// <returns>True if the user has triggered an action, otherwise false</returns>
		bool isTriggered() {
			// Trigger whenever user types T-4-1/2
			if (useSmartTrigger) {
				if (Helpers.IsOneKeyDown(quickChatCodes))
				{
					if (smartTriggerStage != 1)
						Logger.log("SmartTrigger stage is now 1");
					
					smartTriggerStage = 1;
					Helpers.RestartStopwatch(smartTriggerTimeoutWatch);
				}
				if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_4) && smartTriggerStage == 1 && smartTriggerTimeoutWatch.ElapsedMilliseconds < smartTriggerTimeout)
				{
					if (smartTriggerStage != 2)
						Logger.log("SmartTrigger stage is now 2");
					smartTriggerStage = 2;
					Helpers.RestartStopwatch(smartTriggerTimeoutWatch);
				}
				if (InputSimulator.IsKeyDown(VirtualKeyCode.VK_1) || InputSimulator.IsKeyDown(VirtualKeyCode.VK_2))
				{
					if (smartTriggerStage == 2 && smartTriggerTimeoutWatch.ElapsedMilliseconds < smartTriggerTimeout)
					{
						Logger.log("SmartTrigger is now triggered");
						smartTriggerStage = 0;
						return true;
					}
				}
			}
			// Trigger when user presses set key
			else {
				if (Helpers.IsOneKeyDown(simpleTriggerCodes))
				{
					Logger.log("Simple trigger is now triggered");
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>
		/// Performs the action, querying the state of the aircraft and typing the message
		/// </summary>
		/// <returns>False if an error occured, otherwise true</returns>
		bool performCoverMe() {
			try {
				Logger.log("Querying JSON-file for indicators");
				
				string content = RemoteFile.Fetch("http://localhost:8111/indicators");

				Logger.log("Succesfully retrieved JSON-file");
				
				Logger.log("Deserializing JSON");
				
				IndicatorsData data = IndicatorsData.FromJsonString(content);
				
				Logger.log("Done Deserializing");
				
				if (!data.isValid())
				{
					Logger.log("Data is invalid, aborting");
					return false;
				}
				
				int i_alt_in_m  = data.getAltitudeMeters();
				int i_alt_in_ft = (int)(i_alt_in_m / Helpers.METER_PER_FEET);
				
				Logger.log("-> Altitude_Meter = " + i_alt_in_m.ToString());
				Logger.log("-> Altitude_Feet  = " + i_alt_in_ft.ToString());
				
				if(!data.isGoodAltiude())
				{
					Logger.log("Altitude is exceeding the reasonable range, aborting");
					return false;
				}
				
				StringBuilder sb = new StringBuilder();
				sb.Append(Helpers.RoundPreDecimal(i_alt_in_m, 2));
				sb.Append(" m / ");
				sb.Append(Helpers.RoundPreDecimal(i_alt_in_ft, 2));
				sb.Append(" ft");
				
				if (typeHeading) {
					int hdg = data.getHeading();
					
					Logger.log("-> Heading        = " + hdg.ToString());
					
					if (hdg != -1)
					{
						sb.Append(" HDG ");
						sb.Append(hdg);
					}
					else
					{
						Logger.log("Warning! Heading is -1, not appending");
					}
				}
				else
				{
					Logger.log("-> Heading        = Disabled");
				}
				
				Logger.log("Sending virtual keystrokes");
				
				if ((errorCode = SpecialKeyHelper.SimulateLongReturnPress()) != 0)
				{
					Logger.log("Could not insert InputEvents, ErrorCode " + errorCode.ToString());
					return false;
				}
				
				WindowsInput.InputSimulator.SimulateTextEntry(sb.ToString());
				
				if ((errorCode = SpecialKeyHelper.SimulateLongReturnPress()) != 0)
				{
					Logger.log("Could not insert InputEvents, ErrorCode " + errorCode.ToString());
					return false;
				}
				
				return true;
			}
			catch (Exception ex) {
				Logger.logException("Exception while fetching data", ex);
				
				return false;
			}
		}

		
		void BackgroundWorkerDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			Logger.log("Starting to perform background-work");
			for (int numberOfTry = 1; numberOfTry <= MAXIMUM_TRIES; ++numberOfTry) {
				if (!performCoverMe())
					Logger.log("Try " + numberOfTry.ToString() + " failed");
				else
					break;
			}
			Logger.log("Finishing background-work");
		}
	}
}
