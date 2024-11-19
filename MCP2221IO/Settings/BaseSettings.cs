using System;

namespace MCP2221IO.Settings
{
    public abstract class BaseSettings
    {
        internal const int MaxUsbMa = 500;

        internal int _powerRequestMa;

        /// <summary>
        /// The clock duty cycle
        /// </summary>
        public ClockDutyCycle ClockDutyCycle { get; set; }

        /// <summary>
        /// The current clock out divider value
        /// </summary>
        public ClockOutDivider ClockDivider { get; set; }

        /// <summary>
        /// DAC Reference Vrm voltage option
        /// </summary>
        public VrmRef DacRefVrm { get; set; }

        /// <summary>
        /// DAC reference option
        /// </summary>
        public DacRefOption DacRefOption { get; set; }

        /// <summary>
        /// Power up DAC Output Value
        /// </summary>
        public byte DacOutput { get; set; }

        /// <summary>
        /// ADC Reference Vrm voltage option
        /// </summary>
        public VrmRef AdcRefVrm { get; set; }

        /// <summary>
        /// ADC reference option
        /// </summary>
        public AdcRefOption AdcRefOption { get; set; }

        /// <summary>
        /// If set, the interrupt detection flag will be set when a
        /// negative edge occurs.
        /// </summary>
        public bool InterruptNegativeEdge { get; set; }

        /// <summary>
        /// If set, the interrupt detection flag will be set when a
        /// positive edge occurs.
        /// </summary>
        public bool InterruptPositiveEdge { get; set; }

        /// <summary>
        /// The requested mA value during the USB enumeration
        /// </summary>
        public int PowerRequestMa
        {
            get
            {
                return _powerRequestMa * 2;
            }
            protected set
            {
                if(value > MaxUsbMa)
                {
                    throw new ArgumentOutOfRangeException(nameof(PowerRequestMa), $"Value must be less than {MaxUsbMa}");
                }

                _powerRequestMa = value / 2;
            }
        }
    }
}
