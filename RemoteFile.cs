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
using System.IO;
using System.Net;

namespace WT_Logger
{
	/// <summary>
	/// Simple class to obtain a file from a remote host
	/// </summary>
	public class RemoteFile
	{
		
		private RemoteFile() {}
		
		/// <summary>
		/// Reads a file from a remote location and returns its content as text.
		/// </summary>
		/// <param name="Uri">The Uniform Resource Identifier for the file</param>
		/// <returns>The content of the file</returns>
		public static string Fetch(String Uri)
		{
			// Request the file using the Uri and content type
			WebRequest webRequest = WebRequest.Create(Uri);
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.ContentLength = 0;
            
			// Obtain the WebResponse and read it
			WebResponse response = webRequest.GetResponse ();
			Stream stream = response.GetResponseStream ();
			StreamReader reader = new StreamReader (stream);
			string responseFromServer = reader.ReadToEnd ();
            
			// Close everything
			reader.Close ();
			stream.Close ();
			response.Close ();
            
			return responseFromServer;
		}
	}
}
