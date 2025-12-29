using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThuocGiaThatAdmin.Domain.Entities
{
    public class Province
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Code { get; set; }

        public string? Type { get; set; }

        public int CountryId { get; set; }

        public int SortOrder { get; set; }

        public string? ZipCode { get; set; }

        public string? PhoneCode { get; set; }

        public int? IsStatus { get; set; }

        /// <summary>
        /// Miền: B (Bắc), T (Trung), N (Nam)
        /// </summary>
        [MaxLength(1)]
        public string? Region { get; set; }

        // Navigation properties
        public Country Country { get; set; }
        public ICollection<Ward> Wards { get; set; }
    }
}
