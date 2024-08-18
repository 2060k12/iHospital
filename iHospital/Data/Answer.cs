using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iHospital.Data
{

    [Serializable]

    // This class will store the information of the answer
    public class Answer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int? OptionId { get; set; }
        public int RespondantId { get; set; }
        public string AnswerText { get; set; }
    }
}