using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace erpsolution.entities
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string MacAdds { get; set; }

    }
    public class LoginNFCModel
    {
        [Required]
        public string NfcCode { get; set; }
        [Required]
        public string MacAdds { get; set; }
    }
}
