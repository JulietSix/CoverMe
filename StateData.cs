/* 
 * CoverMe! - Copyright (C) 2015 Juliet_Six
 * 
 * This file is part of "CoverMe!". "CoverMe!" is licensed under
 * the Microsoft Public License. You should have received a copy
 * of this license together with this file. Otherwise, see 
 * http://www.microsoft.com/en-us/openness/licenses.aspx
 *
 */
 
// Example on obtaining data from state file
// Does not get build

using System;
using Newtonsoft.Json;

namespace CoverMe
{
	public class StateData
	{
		// Since "AoS, deg" cannot be a field name in C#, make a field of
		// different name and assign its JSON name as a property
		[JsonProperty("AoS, deg")]
		public double AoS = Double.MaxValue;
		
		private StateData() {}
		
		public static StateData FromJsonString(String jsonObject) {
			return JsonConvert.DeserializeObject<StateData>(jsonObject);
		}
		
		public bool IsSlipAngleInValidRange()
		{
			// If AoS was not assigned a value from JSON it is still
			// MaxValue and therefor not valid
			return AoS != Double.MaxValue;
		}
		
		public double GetSlipAngle()
		{
			return AoS;
		}
	}
}
