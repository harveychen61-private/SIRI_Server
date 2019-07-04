using System.Collections.Generic;

namespace SIRI.Models
{

    public class Repository
    {
        public static IDictionary<int, Siri> Siris { get; set; }

        static Repository()
        {
            Siris = new Dictionary<int, Siri>();

        }

    }

}