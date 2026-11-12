using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.context
{
  public class Fundoo2Context:DbContext
    {
        public Fundoo2Context(DbContextOptions<Fundoo2Context> options) : base(options) { }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<NoteEntity> Notes { get; set; }
    }
}
