using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Models {
    public class UserRefreshToken {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Expires { get; set; }
        public string IpAddress { get; set; }
        public bool IsInvalidated { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }

        [NotMapped]
        public bool IsActive {
            get {
                return Expires < DateTime.Now;
            }
        }
    }
}
