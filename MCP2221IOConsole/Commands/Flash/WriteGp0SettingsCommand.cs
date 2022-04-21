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
using MCP2221IO.Gp;
using System;

namespace MCP2221IOConsole.Commands.Flash
{
    [Command(Description = "Write GP0 Power Up Settings")]
    internal class WriteGp0SettingsCommand : BaseWriteGpSetingsCommand
    {
        public WriteGp0SettingsCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [Option("-d", "The GP0 Power Up Designation", CommandOptionType.SingleValue)]
        public (bool HasValue, Gp0Designation Value) Designation { get; set; }

        protected override int OnExecute(CommandLineApplication app, IConsole console)
        {
            return ExecuteCommand((device) =>
            {
                ApplySettings(device);

                if (!(IsInput.HasValue || Value.HasValue || Designation.HasValue))
                {
                    console.Error.WriteLine("No update values specified");
                    app.ShowHelp();
                }
                else
                {
                    if(Designation.HasValue)
                    {
                        device.GpSettings.Gp0PowerUpSetting.Designation = Designation.Value;
                    }

                    device.WriteGpSettings();

                    console.WriteLine("GP0 Settings Updated");
                }

                return 0;
            });
        }
    }
}