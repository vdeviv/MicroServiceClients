using Clients.Domain.Entities;
using Clients.Domain.Interfaces;
using Clients.Infrastructure.Persistences;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Clients.Infrastructure.Repository
{
    public class ClientRepository : IRepository<Client>
    {
        private readonly DatabaseConnection _db;

        public ClientRepository()
        {
            _db = DatabaseConnection.Instance;
        }

        private Client MapClient(DbDataReader reader)
        {
            // Map básico; si quieres, puedes extender para created/updated
            return new Client
            {
                id = reader.GetInt32("id"),
                first_name = reader.GetString("first_name"),
                last_name = reader.GetString("last_name"),
                nit = reader.IsDBNull("nit") ? string.Empty : reader.GetString("nit"),
                email = reader.IsDBNull("email") ? null : reader.GetString("email"),
                is_deleted = reader.GetBoolean("is_deleted")
                // created_by, created_at, updated_by, updated_at se pueden mapear
                // si los incluyes en el SELECT
            };
        }

        public async Task<Client> Create(Client entity)
        {
            using var connection = _db.GetConnection();
            await connection.OpenAsync();

            const string query = @"
                INSERT INTO clients (
                    first_name,
                    last_name,
                    nit,
                    email,
                    is_deleted,
                    created_by,
                    created_at,
                    updated_by,
                    updated_at
                )
                VALUES (
                    @first_name,
                    @last_name,
                    @nit,
                    @email,
                    @is_deleted,
                    @created_by,
                    @created_at,
                    @updated_by,
                    @updated_at
                );
            ";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@first_name", entity.first_name);
            cmd.Parameters.AddWithValue("@last_name", entity.last_name);
            cmd.Parameters.AddWithValue("@nit", entity.nit);
            cmd.Parameters.AddWithValue("@email", (object?)entity.email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@is_deleted", entity.is_deleted);
            cmd.Parameters.AddWithValue("@created_by", entity.created_by);
            cmd.Parameters.AddWithValue("@created_at", entity.created_at);
            cmd.Parameters.AddWithValue("@updated_by", entity.updated_by);
            cmd.Parameters.AddWithValue("@updated_at", entity.updated_at);

            await cmd.ExecuteNonQueryAsync();
            entity.id = (int)cmd.LastInsertedId;
            return entity;
        }

        public async Task<Client?> GetById(Client entity)
        {
            using var connection = _db.GetConnection();
            await connection.OpenAsync();

            const string query = @"
                SELECT
                    id,
                    first_name,
                    last_name,
                    nit,
                    email,
                    is_deleted
                FROM clients
                WHERE id = @id AND is_deleted = FALSE;
            ";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", entity.id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapClient(reader);
            }
            return null;
        }

        public async Task<IEnumerable<Client>> GetAll()
        {
            var list = new List<Client>();

            using var connection = _db.GetConnection();
            await connection.OpenAsync();

            const string query = @"
                SELECT
                    id,
                    first_name,
                    last_name,
                    nit,
                    email,
                    is_deleted
                FROM clients
                WHERE is_deleted = FALSE
                ORDER BY last_name ASC, first_name ASC;
            ";

            using var cmd = new MySqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(MapClient(reader));
            }
            return list;
        }

        public async Task Update(Client entity)
        {
            using var connection = _db.GetConnection();
            await connection.OpenAsync();

            const string query = @"
                UPDATE clients 
                SET first_name = @first_name,
                    last_name  = @last_name,
                    nit        = @nit,
                    email      = @email,
                    updated_by = @updated_by,
                    updated_at = @updated_at
                WHERE id = @id;
            ";

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@first_name", entity.first_name);
            cmd.Parameters.AddWithValue("@last_name", entity.last_name);
            cmd.Parameters.AddWithValue("@nit", entity.nit);
            cmd.Parameters.AddWithValue("@email", (object?)entity.email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@updated_by", entity.updated_by);
            cmd.Parameters.AddWithValue("@updated_at", entity.updated_at);
            cmd.Parameters.AddWithValue("@id", entity.id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task Delete(Client entity)
        {
            const string query = @"
                UPDATE clients
                SET is_deleted = TRUE,
                    updated_by = @updated_by,
                    updated_at = @updated_at
                WHERE id = @id;
            ";

            using var connection = _db.GetConnection();
            await connection.OpenAsync();

            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@updated_by", entity.updated_by);
            cmd.Parameters.AddWithValue("@updated_at", entity.updated_at);
            cmd.Parameters.AddWithValue("@id", entity.id);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
