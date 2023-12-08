using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DAL
{
    public enum SerializationType
    {
        JSON,
        Binary,
        XML,
        Custom
    }

    public class EntityContext<T> where T : new()
    {
        private string connection;
        private SerializationType type;

        public EntityContext(SerializationType type, string connection)
        {
            this.connection = connection;
            this.type = type;
        }

        public void Write(List<T> data)
        {
            if (type == SerializationType.JSON)
            {
                JSONSerialization<T>.Write(data, connection);
            }
            else if (type == SerializationType.Binary)
            {
                BinarySerialization<T>.Write(data, connection);
            }
            else if (type == SerializationType.XML)
            {
                XMLSerialization<T>.Write(data, connection);
            }
            else if (type == SerializationType.Custom)
            {
                CustomSerialization<T>.Write(data, connection);
            }
        }

        public List<T> Read()
        {
            if (type == SerializationType.JSON)
            {
                return JSONSerialization<T>.Read(connection);
            }
            else if (type == SerializationType.Binary)
            {
                return BinarySerialization<T>.Read(connection);
            }
            else if (type == SerializationType.XML)
            {
                return XMLSerialization<T>.Read(connection);
            }
            else
            {
                return CustomSerialization<T>.Read(connection);
            }
        }

        public void Rewrite(List<T> data)
        {
            if (type == SerializationType.JSON)
            {
                JSONSerialization<T>.Rewrite(data, connection);
            }
            else if (type == SerializationType.Binary)
            {
                BinarySerialization<T>.Rewrite(data, connection);
            }
            else if (type == SerializationType.XML)
            {
                XMLSerialization<T>.Rewrite(data, connection);
            }
            else
            {
                CustomSerialization<T>.Rewrite(data, connection);
            }
        }
    }

    [Obsolete]
    public class BinarySerialization<T>
    {
        static public List<T> Read(string connection)
        {
            List<T> data;

            using (FileStream fileStream = new(connection + ".bin", FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();

                try
                {
                    return data = (List<T>)formatter.Deserialize(fileStream);
                }
                catch (Exception e)
                {
                    return new List<T> { };
                }
            }
        }

        static public void Write(List<T> data, string connection)
        {
            using (FileStream fileStream = new(connection + ".bin", FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Serialize(fileStream, data);
            }
        }

        static public void Rewrite(List<T> data, string connection)
        {
            using (FileStream fileStream = new(connection + ".bin", FileMode.Truncate))
            {
                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Serialize(fileStream, data);
            }
        }
    }

    public class JSONSerialization<T>
    {
        private static JsonSerializerOptions options = new() { WriteIndented = true };

        static public List<T> Read(string connection)
        {
            using (FileStream fileStream = new(connection + ".json", FileMode.OpenOrCreate))
            {
                try
                {
                    return (List<T>)JsonSerializer.Deserialize(fileStream, typeof(List<T>), options);
                }
                catch (System.Text.Json.JsonException ex)
                {
                    return new List<T>();
                }
            }
        }

        static public void Write(List<T> data, string connection)
        {
            using (FileStream fileStream = new(connection + ".json", FileMode.OpenOrCreate))
            {
                JsonSerializer.Serialize<List<T>>(fileStream, data, options);
            }
        }

        static public void Rewrite(List<T> data, string connection)
        {
            using (FileStream fileStream = new(connection + ".json", FileMode.Truncate))
            {
                JsonSerializer.Serialize<List<T>>(fileStream, data, options);
            }
        }
    }

    public class XMLSerialization<T>
    {
        static public List<T> Read(string connection)
        {
            using (FileStream fileStream = new(connection + ".xml", FileMode.OpenOrCreate))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(List<T>));
                try
                {
                    return (List<T>)formatter.Deserialize(fileStream);
                }
                catch (Exception e)
                {
                    return new List<T> { };
                }
            }
        }

        static public void Write(List<T> data, string connection)
        {
            using (FileStream fileStream = new(connection + ".xml", FileMode.OpenOrCreate))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(List<T>));
                formatter.Serialize(fileStream, data);
            }
        }

        static public void Rewrite(List<T> data, string connection)
        {
            using (FileStream fileStream = new(connection + ".xml", FileMode.Truncate))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(List<T>));
                formatter.Serialize(fileStream, data);
            }
        }
    }

    public class CustomSerialization<T> where T : new()
    {
        public struct Data
        {
            public string PropertyName;
            public string Value;
        }

        static private List<string> ExtractData(string text, string startString = "<?", string endString = "?>", bool raw = false)
        {
            var matched = new List<string>();
            var exit = false;
            while (!exit)
            {
                var indexStart = text.IndexOf(startString, StringComparison.Ordinal);
                var indexEnd = text.IndexOf(endString, StringComparison.Ordinal);
                if (indexStart != -1 && indexEnd != -1)
                {
                    if (raw)
                        matched.Add("<?" + text.Substring(indexStart + startString.Length,
                                        indexEnd - indexStart - startString.Length) + "?>");
                    else
                        matched.Add(text.Substring(indexStart + startString.Length,
                            indexEnd - indexStart - startString.Length));
                    text = text.Substring(indexEnd + endString.Length);
                }
                else
                {
                    exit = true;
                }
            }
            return matched;
        }

        static private List<Data> ExtractValuesFromData(string text)
        {
            var listOfData = new List<Data>();
            var allData = ExtractData(text, "[", "]");
            foreach (var data in allData)
            {
                var pName = data.Substring(0, data.IndexOf("=", StringComparison.Ordinal));
                var pValue = data.Substring(data.IndexOf("=", StringComparison.Ordinal) + 1);
                listOfData.Add(new Data { PropertyName = pName, Value = pValue });
            }
            return listOfData;
        }

        static private string Serialize(object obj)
        {
            var sb = new StringBuilder();
            var myType = obj.GetType();
            sb.Append("<?");

            if (myType == typeof(Student))
            {
                sb.AppendLine();
                sb.Append("{Student}");
            }
            else if (myType == typeof(Acrobat))
            {
                sb.AppendLine();
                sb.Append("{Acrobat}");
            }
            else if (myType == typeof(TaxiDriver))
            {
                sb.AppendLine();
                sb.Append("{TaxiDriver}");
            }

            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
            foreach (var prop in props)
            {
                var propValue = prop.GetValue(obj, null);
                sb.AppendLine();
                sb.Append(@"    [" + prop.Name + "=" + propValue + "]");
            }
            sb.AppendLine();
            sb.Append("?>");
            return sb.ToString();
        }

        static private string GetType(string data)
        {
            Regex regex = new Regex(@"\{(.+?)\}");
            Match match = regex.Match(data);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return string.Empty;
        }

        static private T DeSerialize(string serializeData)
        {
            var deserializedObjects = ExtractData(serializeData);
            string entity = GetType(deserializedObjects[0]);

            var EntityType = Type.GetType("DAL." + entity);
            T target = (T)Activator.CreateInstance(EntityType);

            foreach (var obj in deserializedObjects)
            {
                var properties = ExtractValuesFromData(obj);
                foreach (var property in properties)
                {
                    var propInfo = target.GetType().GetProperty(property.PropertyName);
                    propInfo?.SetValue(target,
                        Convert.ChangeType(property.Value, propInfo.PropertyType), null);
                }
            }

            return target;
        }

        static public List<T> Read(string connection)
        {
            List<T> data;

            using (FileStream fs = new FileStream(connection + ".custom.txt", FileMode.OpenOrCreate))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    try
                    {
                        string serializedData = reader.ReadToEnd();

                        if (serializedData == "") throw new Exception();

                        string[] values = serializedData.Split("<;>");
                        data = new List<T>();

                        foreach (string value in values)
                        {
                            data.Add(DeSerialize(value));
                        }
                    }
                    catch
                    {
                        data = new List<T>();
                    }
                }
            }

            return data;
        }

        static public void Write(List<T> data, string connection)
        {
            using (FileStream fs = new FileStream(connection + ".custom.txt", FileMode.OpenOrCreate))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    foreach (T item in data)
                    {
                        string serializedData = Serialize(item);
                        string ending = "<;>";

                        if (data.IndexOf(item) == data.Count - 1) ending = "<.>";

                        writer.WriteLine(serializedData + ending);
                    }
                }
            }
        }

        static public void Rewrite(List<T> data, string connection)
        {
            using (FileStream fs = new FileStream(connection + ".custom.txt", FileMode.Truncate))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    foreach (T item in data)
                    {
                        string serializedData = Serialize(item);
                        string ending = "<;>";

                        if (data.IndexOf(item) == data.Count - 1) ending = "<.>";

                        writer.WriteLine(serializedData + ending);
                    }
                }
            }
        }

    }
}
