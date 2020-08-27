using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLParser;
using System.Collections;
using Oracle.ManagedDataAccess.Client;

namespace ParseXML
{
    class Program
    {
        static void Main(string[] args)
        {
            String URLString = "https://www.tcmb.gov.tr/kurlar/today.xml";
            XmlTextReader reader = new XmlTextReader(URLString);
            ArrayList CurrencyArrayList = new ArrayList();
            string user_id = "SYSTEM";
            string user_password = "1234";
            string data_source = "localhost:1521 / xe";

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

            //Create a connection to Oracle
            //string conString = "User Id=hr; password=hr;" +
            string conString = "User Id=" + user_id + "; password=" + user_password +";" +

            //How to connect to an Oracle DB without SQL*Net configuration file
            //also known as tnsnames.ora.
            "Data Source=" + data_source + "; Pooling=false;";
            //"Data Source=localhost:1521/pdborcl; Pooling=false;";

            //How to connect to an Oracle Database with a Database alias.
            //Uncomment below and comment above.
            //"Data Source=pdborcl;Pooling=false;";

            OracleConnection con = new OracleConnection();
            con.ConnectionString = conString;
            con.Open();
            
            Console.WriteLine("Press 1 to Show Values\nPress 2 to Insert Values");
            string userChoice = Console.ReadLine();
            if(userChoice == "1")
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "select * from currency";

                OracleDataReader odr = cmd.ExecuteReader();
                while (odr.Read())
                {
                    Console.WriteLine("Result: " + odr.GetString(0)); //prints currency name only
                }
                Console.WriteLine("Read succesfull.");
            }
            else if(userChoice == "2")
            {
                foreach (CurrencyData cd in CurrencyArrayList)
                {
                    OracleCommand cmd = con.CreateCommand();
                    cmd.CommandText = "insert into currency" +
                    "(cname, unit, forexbuy, forexsell, banknotebuy, banknotesell)" +
                    "values" + "('" + 
                    cd.currencyName + "', " + 
                    cd.unit + ", " +
                    cd.forexBuying + "," +
                    cd.forexSelling + "," +
                    cd.banknoteBuying + "," +
                    cd.banknoteSelling + ")";

                    OracleDataReader odr = cmd.ExecuteReader();
                }
                Console.WriteLine("Insert succesfull.");
            }
            else
            {
                Console.WriteLine("Invalid key pressed.");
            }
        }
    }
}
