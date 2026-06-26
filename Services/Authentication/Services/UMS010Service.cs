

using Application;

using Authentication.Repositories;

using static Authentication.Models.NewFolder.UMS010;

namespace Authentication.Services
{
    public partial interface IUMS010Service
    {
        Task<List<UMS010_Search_Result>> UMS010_Search(UMS010_Search_Criteria criteria);

        Task<GetUserID_Info_Result?> GetUserID_Info(GetUserID_Info_Criteria criteria);

        Task<UMS010_CreateUser_Result> CreateUser(UMS010_CreateUser_Criteria criteria);


        Task<UMS010_UpdateUser_Result> UpdateUser(UMS010_UpdateUser_Criteria criteria);

        Task<UMS010_DeleteUser_Result> DeleteUser(UMS010_DeleteUser_Criteria criteria);

        Task<UMS010_UpdateUser_Result> AdminChangePassword(UMS010_AdminChangePassword_Criteria criteria);

        Task<UMS010_UpdateUser_Result> ChangePassword(UMS010_ChangePassword_Criteria criteria);

        Task<UMS010_ForgotPassword_Result> ForgotPassword(UMS010_ForgotPassword_Criteria criteria);


        //ask<UMS010_CreateUser_Result> BackOfficeCreateUser(UMS010_CreateUserMes_Criteria criteria);



        Task<UMS010_FistLogin_Result> FistLogin(UMS010_FistLogin_Criteria criteria);



    }
    public class UMS010Service : IUMS010Service
    {
        private readonly IUMS010Repository _repository;



        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UMS010Service(IUMS010Repository repository, ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _db = db;

            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<List<UMS010_Search_Result>> UMS010_Search(UMS010_Search_Criteria criteria)
        {
            return await _repository.UMS010_Search(criteria);
        }

        public async Task<GetUserID_Info_Result?> GetUserID_Info(GetUserID_Info_Criteria criteria)
        {
            return await _repository.GetUserID_Info(criteria);
        }


        public int GetCustomerIdFromHeader()
        {
            var context = _httpContextAccessor.HttpContext;

            if (context != null && context.Request.Headers.TryGetValue("X-CustomerID", out var customerIdHeader))
            {
                return Convert.ToInt16(customerIdHeader); // ดึงค่า Header ออกมา
            }

            return 0;
        }

        public async Task<UMS010_CreateUser_Result> CreateUser(UMS010_CreateUser_Criteria criteria)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {

                var result = await _repository.CreateUser(criteria);



                if (result.StatusCode == "200")
                {
                    var apiResultSendEmail = await _repository.SendEmailFirstLogin(new SendEmailFirstLogin_Criteria
                    {
                        FullName = criteria.FirstName + " " + criteria.LastName,
                        UserName = criteria.UserName,
                        Password = criteria.Password,
                        Email = criteria.Email,
                        FirstLoginToken = result.FirstLoginToken,

                    });
                    result.FirstLoginToken = "";
                    await transaction.CommitAsync();
                }
                else
                {
                    await transaction.RollbackAsync();

                }


                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new UMS010_CreateUser_Result
                {
                    StatusCode = "ERROR",
                    StatusName = "ไม่สำเร็จ",
                    MessageCode = "SERVICE_CREATE_EXCEPTION",
                    MessageName = ex.Message
                };
            }
        }

        public async Task<UMS010_UpdateUser_Result> UpdateUser(UMS010_UpdateUser_Criteria criteria)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {

                var result = await _repository.UpdateUser(criteria);
                if (result.StatusCode == "200")
                    await transaction.CommitAsync();
                else
                    await transaction.RollbackAsync();

                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new UMS010_UpdateUser_Result
                {
                    StatusCode = "ERROR",
                    StatusName = "ไม่สำเร็จ",
                    MessageCode = "SERVICE_UPDATE_EXCEPTION",
                    MessageName = ex.Message
                };
            }
        }



        public async Task<UMS010_DeleteUser_Result> DeleteUser(UMS010_DeleteUser_Criteria criteria)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var result = await _repository.DeleteUser(criteria);

                if (result.StatusCode == "200")
                    await transaction.CommitAsync();
                else
                    await transaction.RollbackAsync();

                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<UMS010_UpdateUser_Result> ChangePassword(UMS010_ChangePassword_Criteria criteria)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var result = await _repository.ChangePassword(criteria);

                if (result.StatusCode == "200")
                    await transaction.CommitAsync();
                else
                    await transaction.RollbackAsync();

                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new UMS010_UpdateUser_Result
                {
                    StatusCode = "ERROR",
                    StatusName = "ไม่สำเร็จ",
                    MessageCode = "SERVICE_CHANGE_PASSWORD_EXCEPTION",
                    MessageName = ex.Message
                };
            }
        }

        public async Task<UMS010_ForgotPassword_Result> ForgotPassword(UMS010_ForgotPassword_Criteria criteria)
        {
            return await _repository.ForgotPassword(criteria);
        }

        public async Task<UMS010_UpdateUser_Result> AdminChangePassword(UMS010_AdminChangePassword_Criteria criteria)
        {
            return await _repository.AdminChangePassword(criteria);
        }


        public async Task<UMS010_FistLogin_Result> FistLogin(UMS010_FistLogin_Criteria criteria)
        {
            return await _repository.FistLogin(criteria);
        }
    }
}
