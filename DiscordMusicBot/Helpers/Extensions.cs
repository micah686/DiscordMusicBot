using System;

namespace Yuna.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Creates a timecode string from a <see cref="TimeSpan" />
        /// </summary>
        /// <param name="ts"><see cref="TimeSpan"/> to make a timecode string</param>
        /// <returns><see cref="TimeSpan"/> to timecode string</returns>
        public static string ToTimecode(this TimeSpan ts)
        {
            var hours = ts.Hours;
            var min = ts.Minutes;
            var sec = ts.Seconds;

            return $"{(hours > 0 ? $"{hours:d2}:" : "")}{min:d2}:{sec:d2}";
        }

        /// <summary>
        /// String timecode to <see cref="TimeSpan"/>
        /// </summary>
        /// <param name="timecode">string to be converted</param>
        /// <returns><see cref="TimeSpan" /> of the timecode, default if bad parse</returns>
        public static TimeSpan ToTimeSpan(this string timecode)
        {
            return !TimeSpan.TryParse(timecode, out var res) ? default : res;
        }
    }
}
