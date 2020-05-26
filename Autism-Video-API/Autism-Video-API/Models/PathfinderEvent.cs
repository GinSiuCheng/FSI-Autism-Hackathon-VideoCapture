using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Autism_Video_API.Models
{
    public class PathfinderEvent
    {
        public string PatientID { get; set; }
        public string TimeStamp { get; set; }
        public string Skill { get; set; }
        public string Target { get; set; }
        public string Result { get; set; }
        public string Comments { get; set; }

        public PathfinderEvent(string patientId, string timeStamp, string skill, string target, string result, string comments)
        {
            this.PatientID = patientId;
            this.TimeStamp = timeStamp;
            this.Skill = skill;
            this.Target = target;
            this.Result = result;
            this.Comments = comments;
        }

        public void Save(string StorageConnectionString)
        {
            var ee = new EventEntity(PatientID, TimeStamp, Skill, Target, Result, Comments, StorageConnectionString);
            var me = new MarkerEntity(PatientID, TimeStamp, TimeStamp, Skill+","+Comments, StorageConnectionString);
        }
    }
}