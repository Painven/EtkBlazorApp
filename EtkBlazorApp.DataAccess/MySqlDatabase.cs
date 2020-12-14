﻿using Dapper;
using EtkBlazorApp.DataAccess.Model;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtkBlazorApp.DataAccess
{
    public class MySqlDatabase : IDatabase
    {
        private readonly IConfiguration configuration;
        
        string ConnectionString => configuration.GetConnectionString("openserver_etk_db");

        public MySqlDatabase(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> GetUserPermission(string login, string password)
        {
            var sb = new StringBuilder()
                .AppendLine("SELECT g.permission")
                .AppendLine("FROM oc_user u")
                .AppendLine("LEFT JOIN oc_user_group g ON u.user_group_id = g.user_group_id")
                .AppendLine("WHERE g.name  LIKE 'etk_app%' AND u.status = 1 AND")
                .AppendLine("username = @login AND password = @password");

            var sql = sb.ToString().Trim();

            string passwordMd5 = CreateFromStringMD5(password);

            var permission = await GetScalar<string, dynamic>(sql, new { login, password = passwordMd5 });

            return permission;
        }

        #region Product

        public async Task<List<ProductEntity>> GetLastAddedProducts(int count)
        {
           
            if(count > 100)
            {
                throw new ArgumentOutOfRangeException("Превышен предел запрашиваемых товаров (100)");
            }

            var sb = new StringBuilder()
                .AppendLine("SELECT p.*, d.name as name, m.name as manufacturer")
                .AppendLine("FROM oc_product p")
                .AppendLine("LEFT JOIN oc_product_description d ON p.product_id = d.product_id")
                .AppendLine("LEFT JOIN oc_manufacturer m ON p.manufacturer_id = m.manufacturer_id")
                .AppendLine("ORDER BY Date(p.date_added) ASC")
                .AppendLine("LIMIT @Limit");

            string sql = sb.ToString();

            var products = await LoadData<ProductEntity, dynamic>(sql, new { Limit = count });

            return products.ToList();
        }

        #endregion

        #region Manufacturer
        public async Task SaveManufacturer(ManufacturerEntity manufacturer)
        {
            string sql = "UPDATE oc_manufacturer SET shipment_period = @shipment_period WHERE manufacturer_id = @manufacturer_id";
            await SaveData(sql, manufacturer);
        }

        public async Task<List<ManufacturerEntity>> GetManufacturers()
        {
            string sql = "SELECT m.*, url.keyword FROM oc_manufacturer m LEFT JOIN oc_url_alias url ON CONCAT('manufacturer_id=', m.manufacturer_id) = url.query ORDER BY name";
            var manufacturers = await LoadData<ManufacturerEntity, dynamic>(sql, new { });
            return manufacturers;
        }
        #endregion

        #region ShopAccount
        public async Task SaveShopAccount(ShopAccountEntity account)
        {
            var sb = new StringBuilder();

            if (account.website_id != 0)
            {
                sb
                .AppendLine("UPDATE etk_app_shop_account")
                .AppendLine($"SET {nameof(account.title)} = @{nameof(account.title)},")
                .AppendLine($"    {nameof(account.uri)} = @{nameof(account.uri)},")
                .AppendLine($"    {nameof(account.ftp_host)} = @{nameof(account.ftp_host)},")
                .AppendLine($"    {nameof(account.ftp_login)} = @{nameof(account.ftp_login)},")
                .AppendLine($"    {nameof(account.ftp_password)} = @{nameof(account.ftp_password)},")
                .AppendLine($"    {nameof(account.db_host)} = @{nameof(account.db_host)},")
                .AppendLine($"    {nameof(account.db_login)} = @{nameof(account.db_login)},")
                .AppendLine($"    {nameof(account.db_password)} = @{nameof(account.db_password)}")
                .AppendLine($"WHERE {nameof(ShopAccountEntity.website_id)} = @{nameof(ShopAccountEntity.website_id)}");
            }
            else
            {
                sb
                    .AppendLine("INSERT INTO etk_app_shop_account (")
                    .AppendLine($"{nameof(account.title)},")
                    .AppendLine($"{nameof(account.uri)},")
                    .AppendLine($"{nameof(account.ftp_host)},")
                    .AppendLine($"{nameof(account.ftp_login)},")
                    .AppendLine($"{nameof(account.ftp_password)},")
                    .AppendLine($"{nameof(account.db_host)},")
                    .AppendLine($"{nameof(account.db_login)},")
                    .AppendLine($"{nameof(account.db_password)}) VALUES (")

                    .AppendLine($"@{nameof(account.title)},")
                    .AppendLine($"@{nameof(account.uri)},")
                    .AppendLine($"@{nameof(account.ftp_host)},")
                    .AppendLine($"@{nameof(account.ftp_login)},")
                    .AppendLine($"@{nameof(account.ftp_password)},")
                    .AppendLine($"@{nameof(account.db_host)},")
                    .AppendLine($"@{nameof(account.db_login)},")
                    .AppendLine($"@{nameof(account.db_password)})");
            }

            string sql = sb.ToString().Trim();
            await SaveData(sql, account);

            account.website_id = await GetScalar<int, dynamic>($"SELECT max({nameof(account.website_id)}) FROM etk_app_shop_account", new { });
        }

        public async Task<List<ShopAccountEntity>> GetShopAccounts()
        {
            var sql = "SELECT * FROM etk_app_shop_account";
            var data = await LoadData<ShopAccountEntity, dynamic>(sql, new { });
            return data;
        }

        public async Task DeleteShopAccounts(int id)
        {
            var sql = $"DELETE FROM etk_app_shop_account WHERE {nameof(ShopAccountEntity.website_id)} = @{nameof(ShopAccountEntity.website_id)}";
            await SaveData<dynamic>(sql, new { website_id = id });
        }
        #endregion
        
        #region Orders
        public async Task<List<OrderEntity>> GetLastOrders(int takeCount, string city = null)
        {
            if (takeCount <= 0 || takeCount >= 500)
            {
                return new List<OrderEntity>();
            }

            var sb = new StringBuilder()
                .AppendLine("SELECT o.*, s.name as order_status")
                .AppendLine("FROM oc_order o")
                .AppendLine("LEFT JOIN oc_order_status s ON o.order_status_id = s.order_status_id");

            if (!string.IsNullOrWhiteSpace(city))
            {
                sb.AppendLine("WHERE o.payment_city = @City");
            }
            sb
                .AppendLine("ORDER BY o.date_added DESC")
                .AppendLine("LIMIT @TakeCount");

            string sql = sb.ToString().Trim();
            var orders = await LoadData<OrderEntity, dynamic>(sql, new { TakeCount = takeCount, City = city });
            return orders;
        }

        public async Task<List<OrderDetailsEntity>> GetOrderDetails(int orderId)
        {
            string sql = "SELECT op.*, p.sku as sku, m.name as manufacturer FROM oc_order_product op " +
                "LEFT JOIN oc_product p ON op.product_id = p.product_id " + 
                "LEFT JOIN oc_manufacturer m ON p.manufacturer_id = m.manufacturer_id " + 
                "WHERE order_id = @Id";

            var details = await LoadData<OrderDetailsEntity, dynamic> (sql, new { Id = orderId });

            return details.ToList();
        }

        public async Task<OrderEntity> GetOrderById(int orderId)
        {
            var sql = $"SELECT * FROM oc_order WHERE order_id = @Id";

            var order = (await LoadData<OrderEntity, dynamic>(sql, new { Id = orderId })).FirstOrDefault();

            return order;
        }
        #endregion

        #region private
        private async Task<List<T>> LoadData<T, U>(string sql, U parameters)
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                var rows = await connection.QueryAsync<T>(sql, parameters);

                return rows.ToList();
            }
        }

        private async Task<T> GetScalar<T, U>(string sql, U parameters)
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                T value = await connection.ExecuteScalarAsync<T>(sql, parameters);

                return value;
            }
        }

        private Task SaveData<T>(string sql, T parameters)
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                return connection.ExecuteAsync(sql, parameters);
            }
        }

        private string CreateFromStringMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }
        #endregion
    }
}
