using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iHospital
{
    [Serializable]

    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public int QuestionOrder { get; set; }
    }
}