using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Entity
{
      public class UserEntity
    {
        [Key]
        public int User_ID { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email{ get; set; }
        [Required]
        public string Password { get; set; }
       
        public string PhoneNumber { get; set; }


    }
}
