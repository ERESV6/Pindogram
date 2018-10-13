using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pindogramApp.Entities
{
    public class Meme
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime DateAdded { get; set; }
        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        public User Author { get; set; }
       
    }
}
