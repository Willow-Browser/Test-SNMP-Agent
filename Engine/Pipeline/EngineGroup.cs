using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

namespace Engine.Pipeline
{
    /// <summary>
    /// Engine group which contains all related objects.
    /// </summary>
    public sealed class EngineGroup
    {
        private OctetString engineId;
        private readonly DateTime start;
        private uint counterNotInTimeWindow;
        private uint counterUnknownEngineId;
        private uint counterUnknownUserName;
        private uint counterDecryptionError;
        private uint counterUnknownSecurityLevel;
        private uint counterAuthenticationFailure;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineGroup"/> class.
        /// </summary>
        public EngineGroup(byte[] id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            engineId = new OctetString(id);
            start = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the engine id.
        /// </summary>
        /// <value>The engine id.</value>
        internal OctetString EngineId
        {
            get { return engineId; }
        }

        /// <summary>
        /// Gets or sets the engine boots.
        /// </summary>
        /// <value>The engine boots.</value>
        [Obsolete("Please use EngineTimeData")]
        internal int EngineBoots { get; set; }

        /// <summary>
        /// Gets the engine time.
        /// </summary>
        /// <value>The engine time.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [Obsolete("Please use EngineTimeData")]
        public int EngineTime { get; set; }

        /// <summary>
        /// Gets the engine time data.
        /// </summary>
        /// <value>
        /// The engine time data. [0] is engine boots, [1] is engine time.
        /// </value>
        public int[] EngineTimeData
        {
            get
            {
                var now = DateTime.UtcNow;
                var seconds = (now - start).Ticks / 10000000;
                var engineTime = (int)(seconds % int.MaxValue);
                var engineReboots = (int)(seconds / int.MaxValue);
                return new[] { engineReboots, engineTime };
            }
        }

        /// <summary>
        /// Verifies if the request comes in time.
        /// </summary>
        /// <param name="currentTimeData">The current time data.</param>
        /// <param name="pastReboots">The past reboots.</param>
        /// <param name="pastTime">The past time.</param>
        /// <returns>
        ///   <c>true</c> if the request is in time window; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInTime(int[] currentTimeData, int pastReboots, int pastTime)
        {
            var currentReboots = currentTimeData[0];
            var currentTime = currentTimeData[1];

            // TODO: RFC 2574 page 27
            if (currentReboots == int.MaxValue)
            {
                return false;
            }

            if (currentReboots != pastReboots)
            {
                return false;
            }

            if (currentTime == pastTime)
            {
                return true;
            }

            return Math.Abs(currentTime - pastTime) <= 150;
        }

        /// <summary>
        /// Gets not-in-time-window counter.
        /// </summary>
        /// <value>
        /// Counter variable.
        /// </value>
        public Variable NotInTimeWindow
        {
            get
            {
                return new Variable(Messenger.NotInTimeWindow, new Counter32(counterNotInTimeWindow++));
            }
        }

        /// <summary>
        /// Gets unknown engine ID counter.
        /// </summary>
        /// <value>
        /// Counter variable.
        /// </value>
        public Variable UnknownEngineId
        {
            get
            {
                return new Variable(Messenger.UnknownEngineId, new Counter32(counterUnknownEngineId++));
            }
        }

        /// <summary>
        /// Gets unknown security name counter.
        /// </summary>
        public Variable UnknownSecurityName
        {
            get
            {
                return new Variable(Messenger.UnknownSecurityName, new Counter32(counterUnknownUserName++));
            }
        }

        /// <summary>
        /// Gets decryption error counter.
        /// </summary>
        public Variable DecryptionError
        {
            get
            {
                return new Variable(Messenger.DecryptionError, new Counter32(counterDecryptionError++));
            }
        }

        /// <summary>
        /// Gets unsupported security level counter.
        /// </summary>
        public Variable UnsupportedSecurityLevel
        {
            get
            {
                return new Variable(Messenger.UnsupportedSecurityLevel, new Counter32(counterUnknownSecurityLevel++));
            }
        }

        /// <summary>
        /// Gets authentication failure counter.
        /// </summary>
        public Variable AuthenticationFailure
        {
            get
            {
                return new Variable(Messenger.AuthenticationFailure, new Counter32(counterAuthenticationFailure++));
            }
        }
    }
}