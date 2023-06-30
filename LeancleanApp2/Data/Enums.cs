using System;
using System.Collections.Generic;
using System.Text;

namespace LeancleanApp2.Data
{
    public static class Enums
    {
        public enum LoginFlowStatus
        {
            EmailIsValidated,
            InputEmail,
            RequestEmailValidation,
            WaitingForEmailValidation
        }
    }
}
