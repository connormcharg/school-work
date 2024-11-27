namespace backend.Classes.State
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="CastleRights" />
    /// </summary>
    public class CastleRights
    {
        /// <summary>
        /// Gets or sets a value indicating whether wks
        /// </summary>
        public bool wks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether bks
        /// </summary>
        public bool bks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether wqs
        /// </summary>
        public bool wqs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether bqs
        /// </summary>
        public bool bqs { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CastleRights"/> class.
        /// </summary>
        /// <param name="original">The original<see cref="CastleRights"/></param>
        public CastleRights(CastleRights original)
        {
            this.wks = original.wks;
            this.bks = original.bks;
            this.wqs = original.wqs;
            this.bqs = original.bqs;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CastleRights"/> class.
        /// </summary>
        /// <param name="wks">The wks<see cref="bool"/></param>
        /// <param name="bks">The bks<see cref="bool"/></param>
        /// <param name="wqs">The wqs<see cref="bool"/></param>
        /// <param name="bqs">The bqs<see cref="bool"/></param>
        [JsonConstructor]
        public CastleRights(bool wks, bool bks, bool wqs, bool bqs)
        {
            this.wks = wks;
            this.bks = bks;
            this.wqs = wqs;
            this.bqs = bqs;
        }
    }
}
