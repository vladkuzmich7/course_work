using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BankSystem.Core.Models.Users;

namespace BankSystem.Core.Repositories
{
    public class UserRepository
    {
        private readonly string _filePath = "users.json";
        private List<UserBase> _users = new List<UserBase>();

        public UserRepository()
        {
            LoadFromFile();
        }

        public void AddUser(UserBase user)
        {
            _users.Add(user);
            SaveToFile();
        }

        public List<UserBase> GetAllUsers()
        {
            return _users;
        }

        public UserBase? GetUser(string login)
        {
            return _users.Find(u => u.Login == login);
        }

        public void SaveToFile()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                // Если есть унаследованные типы:
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() },
                IncludeFields = true
            };
            File.WriteAllText(_filePath, JsonSerializer.Serialize(_users, options));
        }

        public void LoadFromFile()
        {
            if (!File.Exists(_filePath))
                return;
            var options = new JsonSerializerOptions
            {
                // Для поддержки наследования:
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() },
                IncludeFields = true
            };
            try
            {
                var json = File.ReadAllText(_filePath);
                // Если есть наследование, понадобится явное указание типа:
                _users = JsonSerializer.Deserialize<List<UserBase>>(json, options) ?? new List<UserBase>();
            }
            catch
            {
                _users = new List<UserBase>();
            }
        }
    }
}