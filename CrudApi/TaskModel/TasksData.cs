using System.ComponentModel.DataAnnotations;

namespace CrudApi.TaskModel
{
    public class TasksData
    {
        public int Id { get; set; }
        public string TaskTitle { get; set; }
        public string TaskDescription { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Deadline { get; set; }
        public bool TaskStatus { get; set; }


    }
}
