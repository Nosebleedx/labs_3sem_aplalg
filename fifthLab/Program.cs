using System;
using System.Xml;

namespace fifthlab_PA
{
    class Program
    {
        static void Main()
        {
            // Путь к вашему XML файлу
            string xmlFilePath = "C:\\Users\\Дмитрий\\source\\repos\\fifthlab_PA\\XMLFile1.xml";
            try
            {
                // Создаем объект XmlDocument
                XmlDocument xmlDoc = new XmlDocument();

                // Загружаем XML файл
                xmlDoc.Load(xmlFilePath);

                // Преобразуем содержимое XML в строку
                string xmlString = xmlDoc.OuterXml;

                // Выводим результат
                Console.WriteLine(xmlString);
                XmlParser xmlParser = new XmlParser();
                xmlParser.ParseXmlString(xmlString);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        class XmlParser
        {
            public void ParseXmlString(string xmlContent)
            {
                int index = 0;
                while (index < xmlContent.Length)
                {
                    if (xmlContent[index] == '<')
                    {
                        // Начало тега
                        int endIndex = xmlContent.IndexOf('>', index);
                        if (endIndex != -1)
                        {
                            string tag = xmlContent.Substring(index + 1, endIndex - index - 1);
                            if (tag.StartsWith("/"))
                            {
                                // Закрывающий тег
                                Console.WriteLine($"> - закрывающий тег: {tag.Substring(1)}");
                            }
                            else if (tag.EndsWith("/"))
                            {
                                // Пустой элемент
                                Console.WriteLine($"< - открывающий тег: {tag.Substring(0, tag.Length - 1)}");
                                Console.WriteLine($"> - закрывающий тег: {tag.Substring(0, tag.Length - 1)}");
                            }
                            else
                            {
                                // Открывающий тег
                                Console.WriteLine($"< - открывающий тег: {tag}");

                                // Обработка атрибутов
                                int spaceIndex = tag.IndexOf(' ');
                                if (spaceIndex != -1)
                                {
                                    string tagName = tag.Substring(0, spaceIndex);
                                    Console.WriteLine($"Имя тега: {tagName}");

                                    string attributesPart = tag.Substring(spaceIndex + 1);
                                    string[] attributes = attributesPart.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                                    foreach (var attribute in attributes)
                                    {
                                        string[] keyValue = attribute.Split('=');
                                        string attributeName = keyValue[0];
                                        string attributeValue = keyValue.Length > 1 ? keyValue[1].Trim('\"') : "";
                                        Console.WriteLine($"Имя атрибута: {attributeName}");
                                        Console.WriteLine($"Значение атрибута: {attributeValue}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Имя тега: {tag}");
                                }
                            }
                            index = endIndex + 1;
                        }
                        else
                        {
                            // Не удалось найти конец тега, выходим из цикла
                            break;
                        }
                    }
                    else if (char.IsWhiteSpace(xmlContent[index]))
                    {
                        // Пропускаем пробелы и символы новой строки
                        index++;
                    }
                    else
                    {
                        // Начало текста
                        int endIndex = xmlContent.IndexOf('<', index);
                        if (endIndex != -1)
                        {
                            string textContent = xmlContent.Substring(index, endIndex - index).Trim();
                            if (!string.IsNullOrEmpty(textContent))
                            {
                                Console.WriteLine($"Текст тега: {textContent}");
                            }
                            index = endIndex;
                        }
                        else
                        {
                            // Не удалось найти начало следующего тега, выходим из цикла
                            break;
                        }
                    }
                }
            }
        }
    }
}
