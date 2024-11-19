
namespace MCP2221IO.Settings
{
    /// <summary>
    /// Chip Settings Protection Level bits
    /// </summary>
    public enum ChipSecurity
    {
        /// <summary>
        /// Permanently locked.
        /// </summary>
        PermanentlyLockedA = 0b11,
        /// <summary>
        /// Permanently locked.
        /// </summary>
        PermanentlyLockedB = 0b10,
        /// <summary>
        /// Password Protected
        /// </summary>
        PasswordProtection = 0b01,
        /// <summary>
        /// Unprotected
        /// </summary>
        Unprotected = 0b00
    }
}