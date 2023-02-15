using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Security;

namespace BouncyCastle
{
    /// <summary>
    /// Privacy provider for AES 192.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "AES", Justification = "definition")]
    public sealed class BouncyCastleAES192PrivacyProvider : BouncyCastleAESPrivacyProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BouncyCastleAES192PrivacyProvider"/> class.
        /// </summary>
        /// <param name="phrase">The phrase.</param>
        /// <param name="auth">The authentication provider.</param>
        public BouncyCastleAES192PrivacyProvider(OctetString phrase, IAuthenticationProvider auth)
            : base(24, phrase, auth)
        { }

        /// <summary>
        /// Returns a string that represents this object.
        /// </summary>
        public override string ToString() => "AES 192 (BouncyCastle) privacy provider";
    }
}