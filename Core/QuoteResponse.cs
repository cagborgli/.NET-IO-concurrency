using System;
using System.Collections.Generic;

namespace Core
{
    [Serializable]
    public class QuoteResponse
    {
        public QuoteResponse()
        {
            this.QuotesRes = new List<QuoteAttributes>();
        }

        public List<QuoteAttributes> QuotesRes { get; set; }
    }
}