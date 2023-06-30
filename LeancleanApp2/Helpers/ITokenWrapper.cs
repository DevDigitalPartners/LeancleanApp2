using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LeancleanApp2.Helpers
{
    public interface ITokenWrapper
    {
        Task<string> GetToken(HttpResponseMessage response);
    }
}
