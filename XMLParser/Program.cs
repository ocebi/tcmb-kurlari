using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseXML
{
    class Program
    {
        static void Main(string[] args)
        {
            String URLString = "https://www.tcmb.gov.tr/kurlar/today.xml";
            XmlTextReader reader = new XmlTextReader(URLString);

            string date = null;
            bool skipping = true;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        if(String.IsNullOrEmpty(date))
                        {
                            reader.MoveToNextAttribute(); //need to have a better logic
                            date = reader.Value;
                            Console.WriteLine("Tarih: " + date);
                        }

                        if(reader.Name == "CurrencyName")
                        {
                            skipping = false;
                        }
                        else if(reader.Name == "CrossRateUSD")
                        {
                            skipping = true;
                        }

                        if(!skipping)
                        {
                            Console.Write(reader.Name + " ");
                        }
                        break;
                    case XmlNodeType.Text: //Display the text in each element.
                        if(!skipping)
                        {
                            Console.WriteLine(reader.Value + "\n");
                        }
                        
                        break;
                    case XmlNodeType.EndElement: //Display the end of the element.
                        break;
                }
            }

            /*
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        Console.Write("<" + reader.Name);

                        while (reader.MoveToNextAttribute()) // Read the attributes.
                        {
                            Console.Write(" " + reader.Name + "='" + reader.Value + "'");
                        }
                            
                        Console.Write(">");
                        Console.WriteLine(">");
                        break;
                    case XmlNodeType.Text: //Display the text in each element.
                        Console.WriteLine("Text value: " + reader.Value);
                        break;
                    case XmlNodeType.EndElement: //Display the end of the element.
                        Console.Write("</" + reader.Name);
                        Console.WriteLine(">");
                        break;
                }
            }
            */
        }
    }
}
