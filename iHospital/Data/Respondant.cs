using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iHospital.Data
{
    [Serializable]

    // This class will store the information of the respondant
    public class Respondant
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
    }
}
