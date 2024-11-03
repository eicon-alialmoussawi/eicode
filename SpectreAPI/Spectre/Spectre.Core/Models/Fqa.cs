using System;
using System.Collections.Generic;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class Fqa
    {
        public int Id { get; set; }
        public string QuestionEn { get; set; }
        public string QuestionAr { get; set; }
        public string QuestionFr { get; set; }
        public string AnswerEn { get; set; }
        public string AnswerAr { get; set; }
        public string AnswerFr { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
