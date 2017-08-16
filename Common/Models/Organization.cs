using System;
using Json;

namespace Common.Models
{
    /// <summary>
    /// Организация
    /// </summary>
    public class Organization
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        public string TaxId { get; set; }

        /// <summary>
        /// Представление объекта
        /// </summary>
        /// <returns>Строковое представление объекта</returns>
        public override string ToString()
        {
            return $"Наименование: {Name}, ИНН: {TaxId}";
        }
    }
}
