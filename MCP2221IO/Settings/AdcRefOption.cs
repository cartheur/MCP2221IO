namespace MCP2221IO.Settings
{
    /// <summary>
    /// ADC Reference Selection
    /// </summary>
    public enum AdcRefOption
    {
        /// <summary>
        /// ADC reference ADC VRM voltage selection (factory default)
        /// </summary>
        Vdd = 1,
        /// <summary>
        /// ADC reference is VDD
        /// </summary>
        Vrm = 0
    }
}