using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TaskListShare
    {
        public string Id { get; set; } = null!;
        public string TaskListId { get; set; } = null!;
        public string UserId { get; set; } = null!;
    }
}
