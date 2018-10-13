using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pindogramApp.Entities
{
    public class MemeRate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("MemeId")]
        public virtual Meme Meme { get; set; }
        public int? MemeId { get; set; }
        public bool isUpvote { get; set; }
    }
}
