using Microsoft.SqlServer.Server;

namespace WebPortal.Domain.Model
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string BankName { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        public int BankId { get; set; }
    }
}
