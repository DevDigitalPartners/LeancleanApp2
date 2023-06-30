using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Text;

namespace LeancleanApp2.Helpers
{
    public interface IEnviroment
    {
        void SetStatusBarColor(Color color,bool darkStatusBarTint);
    }
}
