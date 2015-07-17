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
using WindowsInput;

namespace CoverMe
{
	/// <summary>
	/// The trigger class is responsible for checking if the User has triggered CoverMe
	/// </summary>
	public class Trigger
	{
		private bool useSmartTrigger;
		private List<VirtualKeyCode> triggerKeys;
		private int smartTriggerStage;
		private Stopwatch smartTriggerTimeoutWatch;
		
		public int smartTriggerTimeout = 1000;
		
		/// <summary>
		/// Constructs a new Trigger object
		/// </summary>
		/// <param name="useSmartTrigger">True if SmartTrigger should be used, false if simple trigger</param>
		/// <param name="triggerKeys">Contains quickchat-keys if useSmartTrigger is true, otherwise a collection of keys triggering the simple trigger</param>
		public Trigger(bool useSmartTrigger, List<VirtualKeyCode> triggerKeys)
		{
			this.useSmartTrigger = useSmartTrigger;
			this.triggerKeys = triggerKeys;
			
			if (useSmartTrigger) {
				smartTriggerTimeoutWatch = Stopwatch.StartNew();
				smartTriggerStage = 0;
			}
		}
		
		/// <summary>
		/// Checks wether the this trigger's criteria are met and it is triggered
		/// </summary>
		/// <returns>True if the user has triggered an action, otherwise false</returns>
		public bool isTriggered() {
			// Trigger whenever user types T-4-1/2
			if (useSmartTrigger) {
				if (Helpers.IsOneKeyDown(triggerKeys))
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
				if (Helpers.IsOneKeyDown(triggerKeys))
				{
					Logger.log("Simple trigger is now triggered");
					return true;
				}
			}
			
			return false;
		}
	}
}
