using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stashbox.AspNetCore.Sample.Entity
{
    public class Character
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public House House { get; set; }

        public string? Planet { get; set; }
    }
}
