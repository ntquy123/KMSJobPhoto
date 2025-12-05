using System;
using System.ComponentModel.DataAnnotations;

namespace erpsolution.entities
{
    public class AmtTodoRequest
    {
        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required]
        public string PlayerId { get; set; }

        [Required]
        public string Status { get; set; }
    }
}
