using System;

namespace Clients.Domain.Entities
{
    public class Client
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string nit { get; set; }
        public string email { get; set; }
        public bool is_deleted { get; set; }
        public int created_by { get; set; } 
        public DateTime created_at { get; set; }
        public int updated_by { get; set; }
        public DateTime updated_at { get; set; }

        public Client(int id, string first_name, string last_name, string nit, string email, bool is_deleted)
        {
            this.id = id;
            this.first_name = first_name;
            this.last_name = last_name;
            this.nit = nit;
            this.email = email;
            this.is_deleted = is_deleted;
        }

        public Client() { } 
    }
}