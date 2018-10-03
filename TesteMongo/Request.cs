using System;
using System.Collections.Generic;

namespace TesteMongo
{
    public class Request
    {
        public Request()
        {
            RequestId = Guid.NewGuid().ToString();
        }
        public string RequestId { get; set; }
        public string teste { get; set; }
        public Pessoa Pessoa { get; set; }
        public ICollection<Pessoa> Pessoas { get; set; }
    }
}
