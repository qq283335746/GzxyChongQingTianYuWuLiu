using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Configuration.Provider;
using DBUtility;

namespace TygaSoft.CustomProvider
{
    public class MsSqlRoleProvider : RoleProvider
    {
        private string _AppName;
        private int _SchemaVersionCheck;
        private string _sqlConnectionString;
        private int _CommandTimeout;

        #region 重写成员

        public override void Initialize(string name, NameValueCollection config)
        {
            // Remove CAS from sample: HttpRuntime.CheckAspNetHostingPermission (AspNetHostingPermissionLevel.Low, SR.Feature_not_supported_at_this_level);
            if (config == null)
                throw new ArgumentNullException("config");

            if (String.IsNullOrEmpty(name))
                name = "SqlRoleProvider";
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", SecurityMessage.GetString(SecurityMessage.RoleSqlProvider_description));
            }
            base.Initialize(name, config);

            _SchemaVersionCheck = 0;

            _CommandTimeout = SecurityUtility.GetIntValue(config, "commandTimeout", 30, true, 0);

            string temp = config["connectionStringName"];
            if (temp == null || temp.Length < 1)
                throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Connection_name_not_specified));
            _sqlConnectionString = SqlConnectionHelper.GetConnectionString(temp, true, true);
            if (_sqlConnectionString == null || _sqlConnectionString.Length < 1)
            {
                throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Connection_string_not_found, temp));
            }

            _AppName = config["applicationName"];
            if (string.IsNullOrEmpty(_AppName))
                _AppName = SecurityUtility.GetDefaultAppName();

            if (_AppName.Length > 256)
            {
                throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_application_name_too_long));
            }

            config.Remove("connectionStringName");
            config.Remove("applicationName");
            config.Remove("commandTimeout");
            if (config.Count > 0)
            {
                string attribUnrecognized = config.GetKey(0);
                if (!String.IsNullOrEmpty(attribUnrecognized))
                    throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_unrecognized_attribute, attribUnrecognized));
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            SecurityUtility.CheckArrayParameter(ref roleNames, true, true, true, 256, "roleNames");
            SecurityUtility.CheckArrayParameter(ref usernames, true, true, true, 256, "usernames");

            bool beginTranCalled = false;
            try
            {
                SqlConnectionHolder holder = null;
                try
                {
                    holder = SqlConnectionHelper.GetConnection(_sqlConnectionString, true);
                    //CheckSchemaVersion(holder.Connection);
                    int numUsersRemaing = usernames.Length;
                    while (numUsersRemaing > 0)
                    {
                        int iter;
                        string allUsers = usernames[usernames.Length - numUsersRemaing];
                        numUsersRemaing--;
                        for (iter = usernames.Length - numUsersRemaing; iter < usernames.Length; iter++)
                        {
                            if (allUsers.Length + usernames[iter].Length + 1 >= 4000)
                                break;
                            allUsers += "," + usernames[iter];
                            numUsersRemaing--;
                        }

                        int numRolesRemaining = roleNames.Length;
                        while (numRolesRemaining > 0)
                        {
                            string allRoles = roleNames[roleNames.Length - numRolesRemaining];
                            numRolesRemaining--;
                            for (iter = roleNames.Length - numRolesRemaining; iter < roleNames.Length; iter++)
                            {
                                if (allRoles.Length + roleNames[iter].Length + 1 >= 4000)
                                    break;
                                allRoles += "," + roleNames[iter];
                                numRolesRemaining--;
                            }
                            //
                            // Note:  ADO.NET 2.0 introduced the TransactionScope class - in your own code you should use TransactionScope
                            //            rather than explicitly managing transactions with the TSQL BEGIN/COMMIT/ROLLBACK statements.
                            //
                            if (!beginTranCalled && (numUsersRemaing > 0 || numRolesRemaining > 0))
                            {
                                (new SqlCommand("BEGIN TRANSACTION", holder.Connection)).ExecuteNonQuery();
                                beginTranCalled = true;
                            }
                            AddUsersToRolesCore(holder.Connection, allUsers, allRoles);
                        }
                    }
                    if (beginTranCalled)
                    {
                        (new SqlCommand("COMMIT TRANSACTION", holder.Connection)).ExecuteNonQuery();
                        beginTranCalled = false;
                    }
                }
                catch
                {
                    if (beginTranCalled)
                    {
                        try
                        {
                            (new SqlCommand("ROLLBACK TRANSACTION", holder.Connection)).ExecuteNonQuery();
                        }
                        catch
                        {
                        }
                        beginTranCalled = false;
                    }
                    throw;
                }
                finally
                {
                    if (holder != null)
                    {
                        holder.Close();
                        holder = null;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private void AddUsersToRolesCore(SqlConnection conn, string usernames, string roleNames)
        {
            SqlCommand cmd = new SqlCommand("dbo.aspnet_UsersInRoles_AddUsersToRoles", conn);
            SqlDataReader reader = null;
            SqlParameter p = new SqlParameter("@ReturnValue", SqlDbType.Int);
            string s1 = String.Empty, s2 = String.Empty;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = CommandTimeout;

            p.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(p);
            cmd.Parameters.Add(CreateInputParam("@ApplicationName", SqlDbType.NVarChar, ApplicationName));
            cmd.Parameters.Add(CreateInputParam("@RoleNames", SqlDbType.NVarChar, roleNames));
            cmd.Parameters.Add(CreateInputParam("@UserNames", SqlDbType.NVarChar, usernames));
            cmd.Parameters.Add(CreateInputParam("@CurrentTimeUtc", SqlDbType.DateTime, DateTime.Now));
            try
            {
                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.Read())
                {
                    if (reader.FieldCount > 0)
                        s1 = reader.GetString(0);
                    if (reader.FieldCount > 1)
                        s2 = reader.GetString(1);
                }
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            switch (GetReturnValue(cmd))
            {
                case 0:
                    return;
                case 1:
                    throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_this_user_not_found, s1));
                case 2:
                    throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_role_not_found, s1));
                case 3:
                    throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_this_user_already_in_role, s1, s2));
            }
            throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_unknown_failure));
        }

        public override string ApplicationName
        {
            get { return _AppName; }
            set
            {
                _AppName = value;

                if (_AppName.Length > 256)
                {
                    throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_application_name_too_long));
                }
            }
        }

        public override void CreateRole(string roleName)
        {
            SecurityUtility.CheckParameter(ref roleName, true, true, true, 256, "roleName");

            string cmdText = "dbo.aspnet_Roles_CreateRole";
            SqlParameter[] parms = {
                                       CreateInputParam("@ApplicationName", SqlDbType.NVarChar, ApplicationName),
                                       CreateInputParam("@RoleName", SqlDbType.NVarChar, roleName),
                                       new SqlParameter("@ReturnValue", SqlDbType.Int)
                                   };
            parms[2].Direction = ParameterDirection.ReturnValue;

            SqlHelper.ExecuteNonQuery(_sqlConnectionString, CommandType.StoredProcedure, cmdText, parms);

            int returnValue = (int)parms[2].Value;

            switch (returnValue)
            {
                case 0:
                    return;

                case 1:
                    throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_role_already_exists, roleName));

                default:
                    throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_unknown_failure));
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            SecurityUtility.CheckParameter(ref roleName, true, true, true, 256, "roleName");

            string cmdText = "dbo.aspnet_Roles_DeleteRole";

            SqlParameter[] parms = {
                                       CreateInputParam("@ApplicationName", SqlDbType.NVarChar, ApplicationName),
                                       CreateInputParam("@RoleName", SqlDbType.NVarChar, roleName),
                                       CreateInputParam("@DeleteOnlyIfRoleIsEmpty", SqlDbType.Bit, throwOnPopulatedRole ? 1 : 0),
                                       new SqlParameter("@ReturnValue", SqlDbType.Int)
                                   };
            parms[3].Direction = ParameterDirection.ReturnValue;

            SqlHelper.ExecuteNonQuery(_sqlConnectionString, CommandType.StoredProcedure, cmdText, parms);

            int returnValue = (int)parms[3].Value;

            if (returnValue == 2)
            {
                throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Role_is_not_empty));
            }

            return (returnValue == 0);
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            SecurityUtility.CheckParameter(ref roleName, true, true, true, 256, "roleName");
            SecurityUtility.CheckParameter(ref usernameToMatch, true, true, false, 256, "usernameToMatch");

            string cmdText = "aspnet_UsersInRoles_FindUsersInRole";
            SqlParameter[] parms = {
                                       CreateInputParam("@ApplicationName", SqlDbType.NVarChar, ApplicationName),
                                       CreateInputParam("@RoleName", SqlDbType.NVarChar, roleName),
                                       CreateInputParam("@UserNameToMatch", SqlDbType.NVarChar, usernameToMatch),
                                       new SqlParameter("@ReturnValue", SqlDbType.Int)
                                   };
            parms[3].Direction = ParameterDirection.ReturnValue;

            StringCollection sc = new StringCollection();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(_sqlConnectionString, CommandType.StoredProcedure, cmdText, parms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        sc.Add(reader.GetString(0));
                    }
                }
            }

            if (sc.Count < 1)
            {
                int returnValue = (int)parms[3].Value;

                switch (returnValue)
                {
                    case 0:
                        return new string[0];

                    case 1:
                        throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_role_not_found, roleName));

                    default:
                        throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_unknown_failure));
                }
            }
            String[] strReturn = new String[sc.Count];
            sc.CopyTo(strReturn, 0);
            return strReturn;
        }

        public override string[] GetAllRoles()
        {
            string cmdText = "dbo.aspnet_Roles_GetAllRoles";
            SqlParameter[] parms = {
                                     CreateInputParam("@ApplicationName", SqlDbType.NVarChar, ApplicationName),
                                     new SqlParameter("@ReturnValue",SqlDbType.Int)
                                 };
            parms[1].Direction = ParameterDirection.ReturnValue;

            StringCollection sc = new StringCollection();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(_sqlConnectionString, CommandType.StoredProcedure, cmdText, parms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        sc.Add(reader.GetString(0));
                    }
                }
            }

            String[] strReturn = new String[sc.Count];
            sc.CopyTo(strReturn, 0);
            return strReturn;
        }

        public override string[] GetRolesForUser(string username)
        {
            SecurityUtility.CheckParameter(ref username, true, false, true, 256, "username");

            string cmdText = "dbo.aspnet_UsersInRoles_GetRolesForUser";

            SqlParameter[] parms = {
                                       CreateInputParam("@ApplicationName", SqlDbType.NVarChar, ApplicationName),
                                       CreateInputParam("@UserName", SqlDbType.NVarChar, username),
                                       new SqlParameter("@ReturnValue", SqlDbType.Int)
                                   };
            parms[0].Value = ApplicationName;
            parms[1].Value = username;
            parms[2].Direction = ParameterDirection.ReturnValue;

            StringCollection sc = new StringCollection();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(_sqlConnectionString, CommandType.StoredProcedure, cmdText, parms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        sc.Add(reader.GetString(0));
                    }
                }
            }

            if (sc.Count > 0)
            {
                String[] strReturn = new String[sc.Count];
                sc.CopyTo(strReturn, 0);
                return strReturn;
            }

            int returnValue = (int)parms[2].Value;

            switch (returnValue)
            {
                case 0:
                    return new string[0];
                case 1:
                    return new string[0];
                //throw new ProviderException(SR.GetString(SR.Provider_user_not_found));
                default:
                    throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_unknown_failure));
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            string cmdText = "aspnet_UsersInRoles_GetUsersInRoles";

            SqlParameter[] parms = {
                                       CreateInputParam("@ApplicationName", SqlDbType.NVarChar, ApplicationName),
                                       CreateInputParam("@RoleName", SqlDbType.NVarChar, roleName),
                                       new SqlParameter("@ReturnValue", SqlDbType.Int)
                                   };
            parms[2].Direction = ParameterDirection.ReturnValue;

            StringCollection sc = new StringCollection();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(_sqlConnectionString, CommandType.StoredProcedure, cmdText, parms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        sc.Add(reader.GetString(0));
                    }
                }
            }

            if (sc.Count < 1)
            {
                int returnValue = (int)parms[2].Value;

                switch (returnValue)
                {
                    case 0:
                        return new string[0];
                    case 1:
                        throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_role_not_found, roleName));
                }
                throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_unknown_failure));
            }

            String[] strReturn = new String[sc.Count];
            sc.CopyTo(strReturn, 0);
            return strReturn;
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            SecurityUtility.CheckParameter(ref roleName, true, true, true, 256, "roleName");
            SecurityUtility.CheckParameter(ref username, true, false, true, 256, "username");
            if (username.Length < 1)
                return false;

            string cmdText = "dbo.aspnet_UsersInRoles_IsUserInRole";
            SqlParameter[] parms = {
                                       new SqlParameter("@ReturnValue", SqlDbType.Int),
                                       CreateInputParam("@ApplicationName", SqlDbType.NVarChar, ApplicationName),
                                       CreateInputParam("@UserName", SqlDbType.NVarChar, username),
                                       CreateInputParam("@RoleName", SqlDbType.NVarChar, roleName)
                                   };
            parms[0].Direction = ParameterDirection.ReturnValue;
            SqlHelper.ExecuteNonQuery(_sqlConnectionString, CommandType.StoredProcedure, cmdText, parms);
            int iStatus = (int)parms[0].Value;

            switch (iStatus)
            {
                case 0:
                    return false;
                case 1:
                    return true;
                case 2:
                    return false;
                // throw new ProviderException(SR.GetString(SR.Provider_user_not_found));
                case 3:
                    return false; // throw new ProviderException(SR.GetString(SR.Provider_role_not_found, roleName));
            }
            throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_unknown_failure));
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            SecurityUtility.CheckArrayParameter(ref roleNames, true, true, true, 256, "roleNames");
            SecurityUtility.CheckArrayParameter(ref usernames, true, true, true, 256, "usernames");

            bool beginTranCalled = false;
            try
            {
                SqlConnectionHolder holder = null;
                try
                {
                    holder = SqlConnectionHelper.GetConnection(_sqlConnectionString, true);
                    //CheckSchemaVersion(holder.Connection);
                    int numUsersRemaing = usernames.Length;
                    while (numUsersRemaing > 0)
                    {
                        int iter;
                        string allUsers = usernames[usernames.Length - numUsersRemaing];
                        numUsersRemaing--;
                        for (iter = usernames.Length - numUsersRemaing; iter < usernames.Length; iter++)
                        {
                            if (allUsers.Length + usernames[iter].Length + 1 >= 4000)
                                break;
                            allUsers += "," + usernames[iter];
                            numUsersRemaing--;
                        }

                        int numRolesRemaining = roleNames.Length;
                        while (numRolesRemaining > 0)
                        {
                            string allRoles = roleNames[roleNames.Length - numRolesRemaining];
                            numRolesRemaining--;
                            for (iter = roleNames.Length - numRolesRemaining; iter < roleNames.Length; iter++)
                            {
                                if (allRoles.Length + roleNames[iter].Length + 1 >= 4000)
                                    break;
                                allRoles += "," + roleNames[iter];
                                numRolesRemaining--;
                            }
                            //
                            // Note:  ADO.NET 2.0 introduced the TransactionScope class - in your own code you should use TransactionScope
                            //            rather than explicitly managing transactions with the TSQL BEGIN/COMMIT/ROLLBACK statements.
                            //
                            if (!beginTranCalled && (numUsersRemaing > 0 || numRolesRemaining > 0))
                            {
                                (new SqlCommand("BEGIN TRANSACTION", holder.Connection)).ExecuteNonQuery();
                                beginTranCalled = true;
                            }
                            RemoveUsersFromRolesCore(holder.Connection, allUsers, allRoles);
                        }
                    }
                    if (beginTranCalled)
                    {
                        (new SqlCommand("COMMIT TRANSACTION", holder.Connection)).ExecuteNonQuery();
                        beginTranCalled = false;
                    }
                }
                catch
                {
                    if (beginTranCalled)
                    {
                        (new SqlCommand("ROLLBACK TRANSACTION", holder.Connection)).ExecuteNonQuery();
                        beginTranCalled = false;
                    }
                    throw;
                }
                finally
                {
                    if (holder != null)
                    {
                        holder.Close();
                        holder = null;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private void RemoveUsersFromRolesCore(SqlConnection conn, string usernames, string roleNames)
        {
            SqlCommand cmd = new SqlCommand("dbo.aspnet_UsersInRoles_RemoveUsersFromRoles", conn);
            SqlDataReader reader = null;
            SqlParameter p = new SqlParameter("@ReturnValue", SqlDbType.Int);
            string s1 = String.Empty, s2 = String.Empty;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = CommandTimeout;

            p.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(p);
            cmd.Parameters.Add(CreateInputParam("@ApplicationName", SqlDbType.NVarChar, ApplicationName));
            cmd.Parameters.Add(CreateInputParam("@UserNames", SqlDbType.NVarChar, usernames));
            cmd.Parameters.Add(CreateInputParam("@RoleNames", SqlDbType.NVarChar, roleNames));
            try
            {
                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.Read())
                {
                    if (reader.FieldCount > 0)
                        s1 = reader.GetString(0);
                    if (reader.FieldCount > 1)
                        s2 = reader.GetString(1);
                }
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            switch (GetReturnValue(cmd))
            {
                case 0:
                    return;
                case 1:
                    throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_this_user_not_found, s1));
                case 2:
                    throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_role_not_found, s2));
                case 3:
                    throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_this_user_already_not_in_role, s1, s2));
            }
            throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_unknown_failure));
        }

        public override bool RoleExists(string roleName)
        {
            SecurityUtility.CheckParameter(ref roleName, true, true, true, 256, "roleName");

            string cmdText = "dbo.aspnet_Roles_RoleExists";
            SqlParameter[] parms = {
                                       new SqlParameter("@ReturnValue", SqlDbType.Int),
                                       CreateInputParam("@ApplicationName", SqlDbType.NVarChar, ApplicationName),
                                       CreateInputParam("@RoleName", SqlDbType.NVarChar, roleName)
                                   };
            parms[0].Direction = ParameterDirection.ReturnValue;

            SqlHelper.ExecuteNonQuery(_sqlConnectionString, CommandType.StoredProcedure, cmdText, parms);
            int returnValue = (int)parms[0].Value;

            switch (returnValue)
            {
                case 0:
                    return false;
                case 1:
                    return true;
            }
            throw new ProviderException(SecurityMessage.GetString(SecurityMessage.Provider_unknown_failure));
        }

        #endregion

        #region 自定义属性方法

        private int CommandTimeout
        {
            get { return _CommandTimeout; }
        }

        private SqlParameter CreateInputParam(string paramName, SqlDbType dbType, object objValue)
        {
            SqlParameter param = new SqlParameter(paramName, dbType);
            if (objValue == null)
                objValue = String.Empty;
            param.Value = objValue;
            return param;
        }

        private int GetReturnValue(SqlCommand cmd)
        {
            foreach (SqlParameter param in cmd.Parameters)
            {
                if (param.Direction == ParameterDirection.ReturnValue && param.Value != null && param.Value is int)
                    return (int)param.Value;
            }
            return -1;
        }

        #endregion
    }
}
