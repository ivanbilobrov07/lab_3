using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;

namespace Serialization
{
    public class BinarySerialization<T>
    {
        public static void Write(T[] data, string connection)
        {
            T[] dataFromFile = Read(connection);
            T[] resultData = dataFromFile.Concat(data).ToArray();

            using (FileStream fs = new FileStream(connection + ".bin", FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, resultData);
            }
        }

        public static void Write(T data, string connection)
        {
            T[] dataFromFile = Read(connection);
            T[] resultData = dataFromFile.Append(data).ToArray();

            using (FileStream fs = new FileStream(connection + ".bin", FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, resultData);
            }
        }

        public static T[] Read(string connection)
        {
            T[] data;

            using (FileStream fs = new FileStream(connection + ".bin", FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    data = (T[])formatter.Deserialize(fs);
                }
                catch
                {
                    data = new T[0];
                }
            }

            return data;
        }
    }

    public class JSONSerialization<T>
    {
        public static void Write(T[] data, string connection)
        {
            T[] dataFromFile = Read(connection);
            T[] resultData = dataFromFile.Concat(data).ToArray();

            using (FileStream fs = new FileStream(connection + ".json", FileMode.OpenOrCreate))
            {
                DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(T[]));
                formatter.WriteObject(fs, resultData);
            }
        }

        public static void Write(T data, string connection)
        {
            T[] dataFromFile = Read(connection);
            T[] resultData = dataFromFile.Append(data).ToArray();

            using (FileStream fs = new FileStream(connection + ".json", FileMode.OpenOrCreate))
            {
                DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(T[]));
                formatter.WriteObject(fs, resultData);
            }
        } 

        public static T[] Read(string connection)
        {
            T[] data;

            using (FileStream fs = new FileStream(connection + ".json", FileMode.OpenOrCreate))
            {
                DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(T[]));
                try
                {
                    data = (T[])formatter.ReadObject(fs);
                } catch
                {
                    data = new T[0];
                }
            }

            return data;
        }
    }

    public class XMLSerialization<T>
    {
        public static void Write(T[] data, string connection)
        {
            T[] dataFromFile = Read(connection);
            T[] resultData = dataFromFile.Concat(data).ToArray();

            using (FileStream fs = new FileStream(connection + ".xml", FileMode.OpenOrCreate))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T[]));
                formatter.Serialize(fs, resultData);
            }
        }

        public static void Write(T data, string connection)
        {
            T[] dataFromFile = Read(connection);
            T[] resultData = dataFromFile.Append(data).ToArray();

            using (FileStream fs = new FileStream(connection + ".xml", FileMode.OpenOrCreate))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T[]));
                formatter.Serialize(fs, resultData);
            }
        }

        public static T[] Read(string connection)
        {
            T[] data;

            using (FileStream fs = new FileStream(connection + ".xml", FileMode.OpenOrCreate))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(T[]));
                try
                {
                    data = (T[])formatter.Deserialize(fs);
                }
                catch
                {
                    data = new T[0];
                }
            }

            return data;
        }
    }

    public class CustomSerialization<T> where T : new()
    {
        public struct Data
        {
            public string PropertyName;
            public string Value;
        }

        private static List<string> ExtractData(string text, string startString = "<?", string endString = "?>", bool raw = false)
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

        private static List<Data> ExtractValuesFromData(string text)
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

        private static string Serialize(T data)
        {
            var sb = new StringBuilder();
            sb.Append("<?");
            var myType = data.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
            foreach (var prop in props)
            {
                var propValue = prop.GetValue(data, null);
                sb.AppendLine();
                sb.Append(@"    [" + prop.Name + "=" + propValue + "]");
            }
            sb.AppendLine();
            sb.Append("?>");
            return sb.ToString();
        }

        private static T DeSerialize(string serializeData)
        {
            var target = new T();
            var deserializedObjects = ExtractData(serializeData);

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

        public static List<T> Read(string connection)
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

        public static void Write(T data, string connection)
        {
            List<T> dataFromFile = Read(connection);
            List<T> resultData = new List<T>(dataFromFile.Append(data));

            Rewrite(resultData, connection);
        }

        public static void Write(T[] data, string connection)
        {
            List<T> dataFromFile = Read(connection);
            List<T> resultData = new List<T>(dataFromFile.Concat(data));

            Rewrite(resultData, connection);
        }

        public static void Rewrite(List<T> data, string connection)
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
    }

}