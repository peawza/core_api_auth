using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationDB.Models.Application
{
    [Table("tb_UserRole")]
    public class tb_UserRole : IdentityUserRole<string>
    {


    }
}
