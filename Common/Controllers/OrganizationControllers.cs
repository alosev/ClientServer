using System;
using System.Linq;
using Common.Models;
using System.Collections.Generic;
using System.IO;
using Json;

namespace Common.Controllers
{
    /// <summary>
    /// Контроллер организаций
    /// </summary>
    public class OrganizationControllers
    {
        // Список организаций
        private List<Organization> _organizations;

        public OrganizationControllers()
        {
            _organizations = new List<Organization>();
        }

        /// <summary>
        /// Поиск организации
        /// </summary>
        /// <param name="org">Организация</param>
        /// <returns>true в случае если организация найдена в списке</returns>
        public bool Search(Organization org)
        {
            if(org.Name != "" && org.TaxId != "")
            {
                if(_organizations.Where(t => t.Name == org.Name && t.TaxId == org.TaxId).FirstOrDefault() != null)
                {
                    return true;
                }
            }
            else if(org.TaxId == "")
            {
                if (_organizations.Where(t => t.Name == org.Name).FirstOrDefault() != null)
                {
                    return true;
                }
            }
            else if(org.Name == "")
            {
                if (_organizations.Where(t => t.TaxId == org.TaxId).FirstOrDefault() != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Добавление организации в список
        /// </summary>
        /// <param name="org">Организация</param>
        /// <returns>true, если организаци добавлена</returns>
        public bool Add(Organization org)
        {
            if (!Search(org))
            {
                _organizations.Add(org);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Загрузка данных из файла
        /// </summary>
        /// <param name="filename">Путь к файлу</param>
        public void Load(string filename)
        {
            if (!File.Exists(filename)) { return; }

            _organizations.Clear();

            using (StreamReader sr = new StreamReader(filename))
            {
                while (!sr.EndOfStream)
                {
                    _organizations.Add(JsonParser.Deserialize<Organization>(sr.ReadLine()));
                }
            }
        }

        /// <summary>
        /// Сохранение данных в файл
        /// </summary>
        /// <param name="filename">Путь к файлу</param>
        public void Save(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                foreach (var org in _organizations)
                {
                    sw.WriteLine(JsonParser.Serialize<Organization>(org));
                }
            }
        }

    }
}
