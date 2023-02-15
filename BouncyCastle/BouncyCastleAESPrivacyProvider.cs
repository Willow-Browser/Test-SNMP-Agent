using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Security;

namespace BouncyCastle
{
    /// <summary>
    /// Privacy provider for AES 128.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "AES", Justification = "definition")]
    public sealed class BouncyCastleAESPrivacyProvider : BouncyCastleAESPrivacyProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BouncyCastleAESPrivacyProvider"/> class.
        /// </summary>
        /// <param name="phrase">The phrase.</param>
        /// <param name="auth">The authentication provider.</param>
        public BouncyCastleAESPrivacyProvider(OctetString phrase, IAuthenticationProvider auth)
            : base(16, phrase, auth)
        { }

        /// <summary>
        /// Returns a string that represents this object.
        /// </summary>
        public override string ToString() => "AES 128 (BouncyCastle) privacy provider";
    }
}