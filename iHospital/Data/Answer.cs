using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iHospital.Data
{
    public class Answer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int? OptionId { get; set; }
        public int RespondantId { get; set; }
        public string AnswerText { get; set; }
    }
}