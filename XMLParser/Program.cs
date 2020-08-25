using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLParser;
using System.Collections;

namespace ParseXML
{
    class Program
    {
        static void Main(string[] args)
        {
            String URLString = "https://www.tcmb.gov.tr/kurlar/today.xml";
            XmlTextReader reader = new XmlTextReader(URLString);
            ArrayList CurrencyArrayList = new ArrayList();

            string date = null;
            bool skipping = true;
            string tempReaderName = "";
            string tempName = "";
            string tempUnit = "";
            string tempForexBuying = "";
            string tempForexSelling = "";
            string tempBanknoteBuying = "";
            string tempBanknoteSelling = "";

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

                        if(reader.Name == "Unit")
                        {
                            skipping = false;
                        }
                        else if(reader.Name == "CrossRateUSD")
                        {
                            skipping = true;
                            CurrencyArrayList.Add(new CurrencyData(tempUnit, tempName, tempForexBuying, tempForexSelling, tempBanknoteBuying, tempBanknoteSelling));
                            //reset temp values
                            tempReaderName = "";
                            tempName = "";
                            tempUnit = "";
                            tempForexBuying = "";
                            tempForexSelling = "";
                            tempBanknoteBuying = "";
                            tempBanknoteSelling = "";
                        }

                        if(!skipping)
                        {
                            tempReaderName = reader.Name;
                        }
                        break;
                    case XmlNodeType.Text: //Display the text in each element.
                        if(!skipping)
                        {
                            if(!String.IsNullOrEmpty(reader.Value))
                            {
                                if (tempReaderName == "Unit")
                                {
                                    tempUnit = reader.Value;
                                }
                                else if (tempReaderName == "Isim")
                                {
                                    tempName = reader.Value;
                                }
                                else if (tempReaderName == "ForexBuying")
                                {
                                    tempForexBuying = reader.Value;
                                }
                                else if (tempReaderName == "ForexSelling")
                                {
                                    tempForexSelling = reader.Value;
                                }
                                else if (tempReaderName == "BanknoteBuying")
                                {
                                    tempBanknoteBuying = reader.Value;
                                }
                                else if (tempReaderName == "BanknoteSelling")
                                {
                                    tempBanknoteSelling = reader.Value;
                                }
                            }
                            
                        }
                        
                        break;
                }
            }
            
            foreach(CurrencyData cd in CurrencyArrayList)
            {
                cd.PrintCurrencyInfo();
            }
            
        }
    }
}
