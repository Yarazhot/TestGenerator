using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Main
{
    public class Printer
    {
        private List<ISerializer> serializers;
        private List<StreamWriter> streams;

        public Printer()
        {
            serializers = new List<ISerializer>();
            streams = new List<StreamWriter>();
        }

        public void AddStream(Stream stream)
        {
            streams.Add(new StreamWriter(stream));
        }

        public void AddStream(string fileName)
        {
            FileStream file = File.Create(fileName);
            streams.Add(new StreamWriter(file));
        }

        public void AddSerializer(ISerializer serializer)
        {
            serializers.Add(serializer);
        }

        public void Print(object data)
        {
            foreach (var serializer in serializers)
            {
                foreach (var stream in streams)
                {
                    serializer.Serialize(data, stream);
                }
                
            }
        }
    }
    public class Former{
        public int getInt(){
            return 42;
        }
    }
}