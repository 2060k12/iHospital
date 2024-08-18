using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iHospital.Data
{
    [Serializable]


    // This class will store the information of the option
    public class Option
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Option_Value { get; set; }
        
    }
}