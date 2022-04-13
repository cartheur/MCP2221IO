﻿/*
* MIT License
*
* Copyright (c) 2022 Derek Goslin https://github.com/DerekGn
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/

using McMaster.Extensions.CommandLineUtils;
using MCP2221IO;
using System;

namespace MCP2221IOConsole.Commands
{
    internal abstract class BaseWriteGpSetingsCommand : BaseCommand
    {
        protected BaseWriteGpSetingsCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [Option("-i", Description = "The GP pin is set as input if set")]
        public (bool HasValue, bool Value) IsInput { get; set; }

        [Option("-o", Description = "The output value at power up when pin is set as output")]
        public (bool HasValue, bool Value) OutputValue { get; set; }

        internal void ApplySettings(IDevice device)
        {
            device.ReadGpSettings();

            if(IsInput.HasValue)
            {
                device.GpSettings.Gp0PowerUpSetting.IsInput = IsInput.Value;
            }

            if (OutputValue.HasValue)
            {
                device.GpSettings.Gp0PowerUpSetting.OutputValue = OutputValue.Value;
            }
        }

        internal bool SettingsApplied()
        {
            return IsInput.HasValue && OutputValue.HasValue;
        }
    }
}