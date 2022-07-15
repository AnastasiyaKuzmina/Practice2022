using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace PracticeApp
{
    // Класс для хранения элементов таблицы Users
    public class UserClass
    {
        public int ID { get; set; }
        public string User { get; set; }
    }
    // Класс для хранения элементов таблицы Roles
    public class RoleClass
    {
        public int ID { get; set; }
        public string Role { get; set; }
    }
    // Класс для хранения имен и ролей пользователей
    public class UserAndRoles
    {
        public string NameOfUser { get; set; }
        public List<string> RolesOfUser { get; set; }
        public UserAndRoles(string N, List<string> ROU)
        {
            NameOfUser = N;
            RolesOfUser = ROU;
        }
    }
    // Класс, содержащий функции для получения информации из таблиц
    public class Info
    {
        public static string connectionString = "Server=(localdb)\\mssqllocaldb;Database=Задание;Trusted_Connection=True;";
        public static List<UserClass> GetUsers()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<UserClass>("SELECT * FROM [Users]").ToList();
            }
        }
        public static List<RoleClass> GetRoles()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<RoleClass>("SELECT * FROM [Roles]").ToList();
            }
        }
    }
       
    class Program
    {
        static void Main(string[] args)
        {
            List<UserClass> AllOfUsers = new List<UserClass>();
            AllOfUsers = Info.GetUsers();
            List<RoleClass> AllOfRoles = new List<RoleClass>();
            AllOfRoles = Info.GetRoles();
            List<UserAndRoles> AllOfUsersAndRoles = new List<UserAndRoles>();
            int Count;
            using (IDbConnection db = new SqlConnection(Info.connectionString))
            {
                foreach(UserClass EachUser in AllOfUsers)
                {
                    AllOfUsersAndRoles.Add(new UserAndRoles(EachUser.User, db.Query<string>("SELECT [Role] FROM (SELECT * FROM (SELECT [UserRole].[ID], [UserRole].[UserID], [Roles].[Role] FROM[UserRole] JOIN[Users] ON[UserRole].[UserID] = [Users].[ID] JOIN[Roles] ON[UserRole].[RoleID] = [Roles].[ID]) AS A WHERE [UserID] = @EUID) AS B", new { EUID = EachUser.ID }).ToList()));
                }
                foreach(UserAndRoles EachOfUAR in AllOfUsersAndRoles)
                {
                    Console.Write($"Роли пользователя {EachOfUAR.NameOfUser}:|");
                    foreach(string ROU in EachOfUAR.RolesOfUser)
                    {
                        Console.Write($"{ROU}|");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
                foreach(RoleClass EachRole in AllOfRoles)
                {
                    Count = Convert.ToInt32(db.ExecuteScalar("SELECT COUNT(*) FROM [UserRole] WHERE [RoleID] = @ERID", new { ERID = EachRole.ID }));
                    Console.WriteLine($"Количество пользователей роли {EachRole.Role}: {Count}");
                }
            } 
           
        }
    }
}