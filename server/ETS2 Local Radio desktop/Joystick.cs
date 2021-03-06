﻿/*
 * Simple Joystick API
 *    Coded by Chris Seto 2010
 *    
 * This code released under the Apache 2.0 license, copyright Chris Seto 2010
 * 
 * https://www.ghielectronics.com/community/forum/topic?id=1296&page=2#msg13766
 * */
using System;
using SlimDX.Direct3D9;
using SlimDX.DirectInput;

namespace ETS2_Local_Radio_server
{
    class SimpleJoystick
    {
        /// 
        /// Joystick handle
        /// 
        private Joystick Joystick;

        public DeviceInstance[] AvailableDevices;

        /// 
        /// Get the state of the joystick
        /// 
        public JoystickState State
        {
            get
            {

                if (Joystick.Acquire().IsFailure)
                    throw new Exception("Joystick failure");

                if (Joystick.Poll().IsFailure)
                    throw new Exception("Joystick failure");

                return Joystick.GetCurrentState();
            }
        }

        /// 
        /// Construct, attach the joystick
        /// 
        public SimpleJoystick(string deviceName = null)
        {
            DirectInput dinput = new DirectInput();

            // Current device in loop
            var current = 0;

            var devices = dinput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly);

            // Search for device
            foreach (DeviceInstance device in devices)
            {
                AvailableDevices = new DeviceInstance[devices.Count];
                AvailableDevices[current] = device;
                // If this is the device we want, use it.
                Log.Write("Device found: " + device.ProductName);
                if (Joystick == null || device.InstanceName == deviceName)
                {
                    try
                    {
                        // Create device   
                        Joystick = new Joystick(dinput, device.InstanceGuid);
                    }
                    catch (DirectInputException)
                    {
                    }
                } 
                current++;
            }

            if (Joystick == null)
                throw new Exception("No joystick found");

            foreach (DeviceObjectInstance deviceObject in Joystick.GetObjects())
            {
                if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                    Joystick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-100, 100);
            }

            // Acquire sdevice
            Joystick.Acquire();
        }

        /// 
        /// Release joystick
        /// 
        public void Release()
        {
            if (Joystick != null)
            {
                Joystick.Unacquire();
                Joystick.Dispose();
            }

            Joystick = null;
        }
    }
}