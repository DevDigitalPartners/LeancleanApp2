using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace LeancleanApp2.Helpers
{
    public static class Theme
    {
        public static void SetTheme()
        {
            var e = DependencyService.Get<IEnviroment>();
            Color standart = Color.FromHex("83C934");
            e?.SetStatusBarColor(standart, false);
        }
    }
}
