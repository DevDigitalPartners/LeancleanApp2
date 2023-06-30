using Sentry;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeancleanApp2.Helpers
{
    public static class AppSentry
    {
        private static SentryClient _sentry;
        private static SentryClient Sentry
        {
            get
            {
                //if (Settings.IsSentryEnabled)
                //{
                if (_sentry == null)
                {
                    Dsn.TryParse(Data.Constants.SENTRY_DSN, out Dsn d);
                    SentryOptions so = new SentryOptions
                    {
                        Debug = true,
                        Dsn = d
                    };
                    _sentry = new SentryClient(so);
                }
                return _sentry;
            }
        }

        public static void CaptureException(Exception e)
        {
            if (Data.Settings.IsSentryEnabled)
            {
                Sentry.CaptureEvent(new SentryEvent(e));
            }
        }
    }
}
