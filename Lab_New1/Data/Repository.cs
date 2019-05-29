using AgentAssignment;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace AgentAssignment.Data
{
    public class Repository
    {
        internal static void ReadFile(string fileName, out ObservableCollection<Agent> agents)
        {
            // Create an instance of the XmlSerializer class and specify the type of object to deserialize.
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Agent>));
            TextReader reader = new StreamReader(fileName);
            // Deserialize all the agents.
            agents = (ObservableCollection<Agent>)serializer.Deserialize(reader);
            reader.Close();
        }

        internal static void SaveFile(string fileName, ObservableCollection<Agent> agents)
        {
            // Create an instance of the XmlSerializer class and specify the type of object to serialize.
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Agent>));
            TextWriter writer = new StreamWriter(fileName);
            // Serialize all the agents.
            serializer.Serialize(writer, agents);
            writer.Close();
        }
    }
}
