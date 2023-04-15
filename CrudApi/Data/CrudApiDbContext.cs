using CrudApi.Models;
using CrudApi.TaskModel;
using Microsoft.EntityFrameworkCore;

namespace CrudApi.Data
{
    public class CrudApiDbContext:DbContext
    {
        public CrudApiDbContext(DbContextOptions option):base (option)
        {

            
        }
        public DbSet<User>User { get; set; }
        public DbSet<TasksData> TasksData { get; set; }
       
    }
}
