using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iHospital.Data
{
    [Serializable]

    public class Option
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Option_Value { get; set; }
        
    }
}