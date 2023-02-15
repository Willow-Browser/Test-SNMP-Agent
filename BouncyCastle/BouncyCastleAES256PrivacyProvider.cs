using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Security;

namespace BouncyCastle
{
    /// <summary>
    /// Privacy provider for AES 256.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "AES", Justification = "definition")]
    public sealed class BouncyCastleAES256PrivacyProvider : BouncyCastleAESPrivacyProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BouncyCastleAES256PrivacyProvider"/> class.
        /// </summary>
        /// <param name="phrase">The phrase.</param>
        /// <param name="auth">The authentication provider.</param>
        public BouncyCastleAES256PrivacyProvider(OctetString phrase, IAuthenticationProvider auth)
            : base(32, phrase, auth)
        { }

        /// <summary>
        /// Returns a string that represents this object.
        /// </summary>
        public override string ToString() => "AES 256 (BouncyCastle) privacy provider";
    }
}