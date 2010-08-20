using System;
using System.Collections.Generic;

namespace QueryExamples
{
    /// <summary>
    /// A wrapper around the static operations on <see cref="DateTime"/> which allows time
    /// to be frozen. The clock begins in the  thawed state; that is, calls to <see cref="Now"/>, 
    /// <see cref="Today"/>, and <see cref="UtcNow"/> return current (non-frozen) values.
    /// </summary>
    /// <remarks>This is mostly code from the xUnit Extensions project: http://xunit.codeplex.com/SourceControl/changeset/view/afbefb714a3d#Main%2fxunit.extensions%2fFreezeClock%2fClock.cs</remarks>
    public static class Clock
    {
        static DateTime? frozenTime = null;

        /// <summary>
        /// Gets a <see cref="DateTime"/> object that is set to the current date and time on this computer,
        /// expressed as the local time.
        /// </summary>
        public static DateTime Now
        {
            get { return frozenTime.HasValue ? frozenTime.Value.ToLocalTime() : DateTime.Now; }
        }

        /// <summary>
        /// Gets the current date.
        /// </summary>
        public static DateTime Today
        {
            get { return Now.Date; }
        }

        /// <summary>
        /// Gets a <see cref="DateTime"/> object that is set to the current date and time on this computer,
        /// expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        public static DateTime UtcNow
        {
            get { return frozenTime.HasValue ? frozenTime.Value.ToUniversalTime() : DateTime.UtcNow; }
        }

        /// <summary>
        /// Freezes the clock with the current time.
        /// Until <see cref="Thaw()"/> is called, all calls to <see cref="Now"/>, <see cref="Today"/>, and
        /// <see cref="UtcNow"/> will return the exact same values.
        /// </summary>
        public static void Freeze()
        {
            FreezeLocal(DateTime.Now);
        }

        /// <summary>
        /// Freezes the clock with the given date and time, considered to be local time.
        /// Until <see cref="Thaw()"/> is called, all calls to <see cref="Now"/>, <see cref="Today"/>, and
        /// <see cref="UtcNow"/> will return the exact same values.
        /// </summary>
        /// <param name="localDateTime">The local date and time to freeze to</param>
        public static void FreezeLocal(DateTime localDateTime)
        {
            if (frozenTime.HasValue)
                throw new InvalidOperationException("Clock is already frozen");

            frozenTime = DateTime.SpecifyKind(localDateTime, DateTimeKind.Local);
        }

        /// <summary>
        /// Freezes the clock with the given date and time, considered to be Coordinated Universal Time (UTC).
        /// Until <see cref="Thaw()"/> is called, all calls to <see cref="Now"/>, <see cref="Today"/>, and
        /// <see cref="UtcNow"/> will return the exact same values.
        /// </summary>
        /// <param name="utcDateTime">The UTC date and time to freeze to</param>
        public static void FreezeUtc(DateTime utcDateTime)
        {
            frozenTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
        }

        /// <summary>
        /// Thaws the clock so that <see cref="Now"/>, <see cref="Today"/>, and <see cref="UtcNow"/>
        /// return normal values.
        /// </summary>
        public static void Thaw()
        {
            if (!frozenTime.HasValue)
                throw new InvalidOperationException("Clock is not frozen");

            frozenTime = null;
        }

        /// <summary>
        /// If the clock is currently frozen
        /// </summary>
        public static bool IsFrozen
        {
            get
            {
                return frozenTime.HasValue;
            }
        }
    }

    public static class Dates
    {
        public static DateRangeBuilder From(DateTime startDate)
        {
            return new DateRangeBuilder(startDate);
        }

        public class DateRangeBuilder
        {
            private readonly DateTime _startDate;

            public DateRangeBuilder(DateTime startDate)
            {
                _startDate = startDate;
            }

            public IEnumerable<DateTime> To(DateTime endDate)
            {
                var current = _startDate;
                while (current < endDate)
                {
                    yield return current;
                    current = current.AddDays(1);
                }
            }
        }

        public static DateTime Today
        {
            get { return Clock.Today; }
        }

        public static DateTime MonthsFromToday(this int monthsFromToday)
        {
            return Clock.Today.AddMonths(monthsFromToday);
        }
    }
}