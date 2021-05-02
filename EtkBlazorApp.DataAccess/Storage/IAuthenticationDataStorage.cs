﻿using EtkBlazorApp.DataAccess.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EtkBlazorApp.DataAccess
{
    public interface IAuthenticationDataStorage
    {
        Task<string> GetUserPermission(string login, string password);
        Task UpdateUserLastLoginDate(string login);

        Task<List<AppUserEntity>> GetUsers();
        Task UpdateUser(AppUserEntity user);
        Task AddUser(AppUserEntity user);
        Task DeleteUser(int user_id);
        Task<List<AppUserGroupEntity>> GetUserGroups();
    }

    public class AuthenticationDataStorage : IAuthenticationDataStorage
    {
        private readonly IDatabaseAccess database;

        public AuthenticationDataStorage(IDatabaseAccess database)
        {
            this.database = database;
        }

        public async Task<string> GetUserPermission(string login, string password)
        {
            var sb = new StringBuilder()
                .AppendLine("SELECT permission")
                .AppendLine("FROM etk_app_user u")
                .AppendLine("LEFT JOIN etk_app_user_group g ON u.user_group_id = g.user_group_id")
                .AppendLine("WHERE u.status = 1 AND login = @login AND password = MD5(@password)");

            var sql = sb.ToString().Trim();

            var permission = await database.GetScalar<string, dynamic>(sql, new { login, password });

            return permission;
        }

        public async Task UpdateUserLastLoginDate(string login)
        {
            await database.SaveData<dynamic>("UPDATE etk_app_user SET last_login_date = NOW() WHERE login = @login", new { login });
        }

        public async Task<List<AppUserEntity>> GetUsers()
        {
            string sql = "SELECT u.*,  g.name as group_name " +
                         "FROM etk_app_user u " +
                         "JOIN etk_app_user_group g ON u.user_group_id = g.user_group_id";

            var users = await database.LoadData<AppUserEntity, dynamic>(sql, new { });

            return users;
        }

        public async Task UpdateUser(AppUserEntity user)
        {
            var sb = new StringBuilder()
                .AppendLine("UPDATE etk_app_user")
                .AppendLine("SET user_group_id = @user_group_id,")
                .AppendLine("ip = @ip,")
                .AppendLine("status = @status");

            if(user.password != null)
            {
                sb.AppendLine(", password = MD5(@password)");
            }

            sb.Append("WHERE user_id = @user_id");

            string sql = sb.ToString();

            await database.SaveData<dynamic>(sql, user);
        }

        public async Task AddUser(AppUserEntity user)
        {
            string sql = "INSERT INTO etk_app_user (login, password, ip, user_group_id, status) VALUES " + 
                         "(@login, MD5(@password), @ip, @group_id, '1')";
            await database.SaveData<dynamic>(sql, user);

        }

        public async Task DeleteUser(int user_id)
        {
            await database.SaveData<dynamic>("DELETE FROM etk_app_user WHERE user_id = @user_id", new { user_id });
        }

        public async Task<List<AppUserGroupEntity>> GetUserGroups()
        {
            var groups = await database.LoadData<AppUserGroupEntity, dynamic>("SELECT * FROM etk_app_user_group", new { });
            return groups;
        }
    }
}
