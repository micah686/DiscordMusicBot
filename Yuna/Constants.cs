namespace Yuna
{
    public static class Constants
    {
        public static readonly string USER_NOT_IN_VOICE = $"{EMOJI_WARNING} You must first be in the same voice channel to execute this command.";
        public static readonly string BOT_NOT_IN_VOICE = $"{EMOJI_WARNING} I must be in a voice channel to execute this command.";
        public static readonly string PLAYER_NOT_FOUND = $"{EMOJI_WARNING} Player was not found.";

        public static readonly string EMOJI_WARNING = "⚠️";
        public static readonly string EMOJI_MUSIC_NOTES = "🎶";
        public static readonly string EMOJI_X = "❌";
        public static readonly string EMOJI_SHUFFLE = "🔀";
        public static readonly string EMOJI_LOOP = "🔂";


        internal static readonly byte[] START_BYTES = new byte[] { 0x00, 0x01,0x02,0x03,0x04,0x05 };
        internal static readonly byte[] END_BYTES = new byte[] { 0xFF, 0xFE, 0xFD, 0xFC, 0xFB, 0xFA };
        internal const string LAUNCHSTATE_FILE = "launchstate";
    }
}
