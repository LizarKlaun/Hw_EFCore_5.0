using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hw_EFCore_5._0.Entities
{
    public class Dog
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Age { get; set; }

        [Required]
        public string Breed { get; set; }

        public bool IsAdopted { get; set; }

        public override string ToString()
        {
            string status = IsAdopted ? "Прилаштовано" : "У притулку";
            return $"ID: {Id,-4} | Кличка: {Name,-15} | Вік: {Age,-3} | Порода: {Breed,-20} | Статус: {status}";
        }
    }
}
