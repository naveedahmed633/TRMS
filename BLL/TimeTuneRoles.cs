using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public static class TimeTuneRoles
    {
        public const string ROLE_HR = DLL.Commons.Roles.ROLE_HR;
        public const string ROLE_EMP = DLL.Commons.Roles.ROLE_EMP;
        public const string ROLE_LM = DLL.Commons.Roles.ROLE_LM;
        public const string ROLE_SLM = DLL.Commons.Roles.ROLE_SLM;

        // This role will not be included in the access_groups table.
        public const string ROLE_SUPER_USER = DLL.Commons.Roles.ROLE_SUPER_USER;
        public const string ROLE_ADMIN = DLL.Commons.Roles.ROLE_ADMIN;
        public const string ROLE_REPORT = DLL.Commons.Roles.ROLE_REPORT;
        public const string ROLE_SUDO = DLL.Commons.Roles.ROLE_SUDO;
    }

    public static class TimeTuneFunction
    {
        public const string FUNCTION_STUDENT = DLL.Commons.FunctionRoles.FUNC_STUDENT;
        public const string FUNCTION_LECTURER = DLL.Commons.FunctionRoles.FUNC_LECTURER;
        public const string FUNCTION_STAFF = DLL.Commons.FunctionRoles.FUNC_STAFF;
    }

}
