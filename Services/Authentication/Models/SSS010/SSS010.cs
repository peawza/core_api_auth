namespace Authentication.Models.SSS010
{
    public class SSS010
    {
        #region GetGroupPermissionUser
        public class SSS010_GetGroupPermissionUser_Criteria
        {
            public string UserId { get; set; }
        }

        public class SSS010_GetGroupPermissionUser_Result
        {
            public string ScreenID { get; set; }
            public int FunctionCode { get; set; }
        }


        #endregion



        public class SSS010_WhereAllData_Criteria
        {

        }

        public class SSS010_WhereAllData_Result
        {
            public string WhereAllData { get; set; }

        }





    }
}
