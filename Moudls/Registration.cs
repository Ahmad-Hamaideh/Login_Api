using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LoginApi.Moudls
{

    public class Registration
    {
      
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string UserPassword { get; set; }

        public DateTime RegistrationDate { get; set; }
        public int IsActive { get; set; }
    }
    


        public class LoginClass
        {
            public string UserEmail { get; set; }
            public string UserPassword { get; set; }
        
    
    }
}

