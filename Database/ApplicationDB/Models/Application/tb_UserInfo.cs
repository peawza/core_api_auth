using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class tb_UserInfo
{
    [Comment("User ID (Main Key)")]
    public string Id { get; set; }

    [Required]
    public int UserNumber { get; set; }

    [Required]
    [Comment("UserName")]
    public string UserName { get; set; }

    [Comment("Employee Code")]
    public string? EmployeeCode { get; set; }

    [Required]
    [Comment("First Name")]
    public string FirstName { get; set; }

    [Required]
    [Comment("Last Name")]
    public string LastName { get; set; }

    [Comment("Remark, more note.")]
    public string? Remark { get; set; }

    [Comment("Language Code login")]
    public string? LanguageCode { get; set; }

    [Comment("Department Code login")]
    public string? DepartmentCode { get; set; }

    [Comment("Position Code login")]
    public string? PositionCode { get; set; }

    [Required]
    [Comment("Create Date")]
    public DateTime CreateDate { get; set; }

    [Required]
    [Comment("Create By")]
    public string CreatedBy { get; set; }

    [Comment("Update Date")]
    public DateTime? UpdateDate { get; set; }

    [Comment("Update By")]
    public string? UpdatedBy { get; set; }

    public bool DeletedFlag { get; set; } = false;

    public DateTime? DeletedDate { get; set; }

    public string? DeletedBy { get; set; }
}
