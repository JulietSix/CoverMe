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
using System.Runtime.InteropServices;
using WindowsInput;

namespace CoverMe
{
	// The following two structures are used to communicate with the Windows APIs
	// written for C.
	
	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct INPUT {
		public UInt32 type;
		public KEYBDINPUT input;
	}
	
	// Size must be 24 as the MOUSEINPUT structure which would be part of the 
	// union "input" in INPUT is 4 bytes bigger than the KEYBDINPUT structure.
	// Since this program does not implement that union and instead always uses
	// the KEYBDINPUT the size of the struct INPUT would be too small and calls
	// to SendInput would fail
	[StructLayout(LayoutKind.Sequential, Pack=1, Size=24)]
	public struct KEYBDINPUT {
		public UInt16 wVk;
		public UInt16 wScan;
		public UInt32 dwFlags;
		public UInt32 time;
		public IntPtr dwExtraInfo;
	}
	
	/// <summary>
	/// SpecialKeyHelper implements the methods that perform native Windows API calls
	/// to simulate the long-press of the Return Key.
	/// </summary>
	public class SpecialKeyHelper
	{				
		public static int StrokeDelay = 50;
		
		public const UInt32 KEYEVENTF_KEYUP = 0x02;
		
		public const UInt32 INPUT_KEYBOARD = 1;
		
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int MapVirtualKey(int uCode, int uMapType);
		
		[DllImport("user32.dll")]
		public static extern UInt32 SendInput(UInt32 nInputs, INPUT[] pInputs, Int32 cbSize);
		
		[DllImport("user32.dll")]
		public static extern IntPtr GetMessageExtraInfo();
		
		[DllImport("kernel32.dll")]
		public static extern UInt32 GetLastError();
		
		[DllImport("kernel32.dll")]
		public static extern void SetLastError(UInt32 dwErrCode);
		
		/// <summary>
		/// Simulates a long press of the return key. Timing depends on the field 'StrokeDelay'
		/// </summary>
		/// <returns>Zero if successful, non-zero on error</returns>
		public static uint SimulateLongReturnPress() {
			
			uint lastError;
			
			if ((lastError = ChangeKeyState(VirtualKeyCode.RETURN, true)) != 0) {
				return lastError;
			}
			
			System.Threading.Thread.Sleep(StrokeDelay);

			if ((lastError = ChangeKeyState(VirtualKeyCode.RETURN, false)) != 0) {
				return lastError;
			}

			System.Threading.Thread.Sleep(StrokeDelay);
			
			return 0;
		}
		
		/// <summary>
		/// Changes the state of a key.
		/// </summary>
		/// <param name="key">Virtual keycode of the key</param>
		/// <param name="pressed">True if key should be pressed, false if released</param>
		/// <returns>Zero on success, otherwise non-zero</returns>
		public static uint ChangeKeyState(VirtualKeyCode key, bool pressed) {
			// Convert VirtualKeyCode to UInt16
			ushort vk = (ushort)key;
			
			// Map virtual key to physical scan-code
			ushort sc = (ushort)MapVirtualKey(vk, 0);
			
			// Fill out INPUT structure
			INPUT mInput;
			mInput.type = INPUT_KEYBOARD;
			mInput.input.wVk = vk;
			mInput.input.wScan = sc;
			// If the key should be pressed, flags are empty, otherwise contain KEYEVENTF_KEYUP
			mInput.input.dwFlags = pressed ? 0 : KEYEVENTF_KEYUP;
			mInput.input.time = 0;
			mInput.input.dwExtraInfo = GetMessageExtraInfo();
			
			// Make array from structure
			INPUT[] mInputs = {mInput};
			
			// Reset LastError variable
			SetLastError(1);
			
			// Perform the Input-insertion and check wether SendInput inserted all INPUT structures from the array
			if (SendInput((UInt32)mInputs.Length, mInputs, Marshal.SizeOf(typeof(INPUT))) != mInputs.Length)
			{
				// If not return the last error code
				return GetLastError();
			}
			
			// Return zero on success
			return 0;
		}
		
		private SpecialKeyHelper() {}
	}
}
