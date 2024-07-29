using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iHospital.Data
{
    [Serializable]

    public class DependentQuestion
    {

        public int Id { get; set; }  
        public int QuestionId { get; set; } 
        public int OptionID { get; set; } 


    }
}