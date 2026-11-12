using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Entity
{
    public class NoteEntity
    {
        [Key]
        public int NoteId { get; set; }
        [Required]
        [MaxLength(200)]
        public string? title { get; set; }
        public string? description { get; set; }
        [MaxLength(30)]
        public string? colour { get; set; }

        public bool isArchived { get; set; }

        public bool isDeleted { get; set; }
        [Required]
        public int UserId { get; set; }
        public UserEntity user { get; set; } = null;



    }
}
