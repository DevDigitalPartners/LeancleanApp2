using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace LeancleanApp2.Data
{
    internal class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        private const string liveUrlKey = "LiveUrl";
        private static readonly string liveUrlDefault = "";

        public static string LiveUrl
        {
            get
            {
                return AppSettings.GetValueOrDefault(liveUrlKey, liveUrlDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(liveUrlKey, value);
            }
        }

        private const string deviceIdKey = "DeviceID";
        private static readonly string deviceIdDefault = "";

        public static string DeviceID
        {
            get
            {
                return AppSettings.GetValueOrDefault(deviceIdKey, deviceIdDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(deviceIdKey, value);
            }
        }

        private const string loginStatusKey = "LoginStatus";
        private static readonly Enums.LoginFlowStatus loginStatusDefault = Enums.LoginFlowStatus.InputEmail;

        public static Enums.LoginFlowStatus LoginStatus
        {
            get
            {
                return (Enums.LoginFlowStatus)AppSettings.GetValueOrDefault(loginStatusKey, (int)loginStatusDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(loginStatusKey, (int)value);
            }
        }

        private const string activeUserKey = "ActiveUser";
        private static readonly string activeUserDefault = "";

        public static string ActiveUser
        {
            get
            {
                return AppSettings.GetValueOrDefault(activeUserKey, activeUserDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(activeUserKey, value);
            }
        }

        private const string appKeyKey = "AppKey";
        private static readonly string appKeyDefault = "";

        public static string AppKey
        {
            get
            {
                return AppSettings.GetValueOrDefault(appKeyKey, appKeyDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(appKeyKey, value);
            }
        }


        private const string isSentryEnabledKey = "IsSentryEnabled";
        private static readonly bool isSentryEnabledDefault = false;

        public static bool IsSentryEnabled
        {
            get
            {
                return AppSettings.GetValueOrDefault(isSentryEnabledKey, isSentryEnabledDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(isSentryEnabledKey, value);
            }
        }

    }
}
