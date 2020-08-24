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
            string tempNameValue = "";

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
                            Console.Write("\n");
                        }
                        else if(reader.Name == "CrossRateUSD")
                        {
                            skipping = true;
                        }

                        if(!skipping)
                        {
                            tempNameValue = reader.Name;
                        }
                        break;
                    case XmlNodeType.Text: //Display the text in each element.
                        if(!skipping)
                        {
                            if(!String.IsNullOrEmpty(reader.Value))
                            {
                                tempNameValue = tempNameValue + ": " + reader.Value;
                                Console.WriteLine(tempNameValue);
                            }
                            
                        }
                        
                        break;
                }
            }
            
        }
    }
}
