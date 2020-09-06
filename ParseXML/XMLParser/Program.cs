using System;
using System.Xml;
using System.Collections.Generic;
using XMLParser;
using System.Collections;
using Oracle.ManagedDataAccess.Client;
using System.ServiceProcess;
using System.Threading;

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
            string tempCrossRateUSD = "";

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        if(String.IsNullOrEmpty(date))
                        {
                            reader.MoveToNextAttribute(); //move to date attribute
                            date = reader.Value;
                            Console.WriteLine("Tarih: " + date);
                        }

                        if(reader.Name == "Unit")
                        {
                            skipping = false; //start reading currency info
                        }
                        else if(reader.Name == "CrossRateOther")
                        {
                            skipping = true; //save currency info
                            CurrencyArrayList.Add(new CurrencyData(date, tempUnit, tempName, tempForexBuying, tempForexSelling, tempBanknoteBuying, tempBanknoteSelling, tempCrossRateUSD));
                            //reset temp values
                            tempReaderName = "";
                            tempName = "";
                            tempUnit = "";
                            tempForexBuying = "";
                            tempForexSelling = "";
                            tempBanknoteBuying = "";
                            tempBanknoteSelling = "";
                            tempCrossRateUSD = "";
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
                                else if(tempReaderName == "CrossRateUSD")
                                {
                                    tempCrossRateUSD = reader.Value;
                                }
                            }
                            
                        }
                        
                        break;
                }
            }

            //Create a connection to Oracle
            //string conString = "User Id=hr; password=hr;" +
            string conString = "User Id=" + user_id + "; password=" + user_password +";" +
            "Data Source=" + data_source + "; Pooling=false;";
            
            OracleConnection con = new OracleConnection();
            con.ConnectionString = conString;
            con.Open();
            
            Console.WriteLine("Press 1 to Show Values\nPress 2 to Insert Values\nPress 3 to Delete Values");
            string userChoice; //= Console.ReadLine();
            
            if(args.Length == 0)
            {
                userChoice = Console.ReadLine();
            }
            else
            {
                userChoice = args[0];
            }
            
            if(userChoice == "1") //show currency names
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "select * from currency";

                OracleDataReader odr = cmd.ExecuteReader();
                while (odr.Read())
                {
                    Console.WriteLine("Result: " + odr.GetString(1)); //prints currency name only
                }
                Console.WriteLine("Read succesfull.");
            }
            else if(userChoice == "2") //insert pulled data
            {
                //check if the data already exists
                CurrencyData tempCurrencyData = (CurrencyData)CurrencyArrayList[0];
                OracleCommand cmd0 = con.CreateCommand();
                cmd0.CommandText = "select * from currency where cdate=to_date('" + tempCurrencyData.date + "' , 'dd.mm.yyyy')";
                OracleDataReader odr0 = cmd0.ExecuteReader(); //check if data already exists in the database
                if(!odr0.HasRows)
                {
                    Console.WriteLine("New data detected. Inserting.");
                    foreach (CurrencyData cd in CurrencyArrayList)
                    {


                        OracleCommand cmd = con.CreateCommand();
                        cmd.CommandText = "insert into currency" +
                        "(cdate, cname, unit, forexbuy, forexsell, banknotebuy, banknotesell, crossrateusd)" +
                        "values" + "(" +
                        "TO_DATE('" + cd.date + "', 'dd.mm.yyyy')" + " , " + //string to date
                        "'" + cd.currencyName + "', " +
                        cd.unit + ", " +
                        cd.forexBuying + "," +
                        cd.forexSelling + "," +
                        cd.banknoteBuying + "," +
                        cd.banknoteSelling + "," +
                        cd.crossRateUSD + ")";

                        OracleDataReader odr = cmd.ExecuteReader();
                    }
                    Console.WriteLine("Insert succesfull.");
                    
                }
                else
                {
                    Console.WriteLine("Data already exists in the database.");
                }

                //restart the service after insertion
                if (args.Length > 0)
                {
                    if (args[0] == "2")
                    {
                        try
                        {
                            Console.WriteLine("Restarting service.");
                            Thread.Sleep(10000); //sleep to make sure service has stopped
                            ServiceController service = new ServiceController("CurrencyReader");
                            service.Start();
                        }
                        catch(Exception e)
                        {
                            throw new Exception("Can not restart the Windows Service CurrencyReader", e);
                        }
                        
                    }
                }
                

                    }
            else if(userChoice == "3") //reset the database
            {
                Console.WriteLine("Delete all rows? y/n");
                userChoice = Console.ReadLine();
                if(userChoice == "y" || userChoice == "Y")
                {
                    OracleCommand cmd = con.CreateCommand();
                    cmd.CommandText = "delete from currency";
                    OracleDataReader odr = cmd.ExecuteReader();
                    Console.WriteLine("All rows deleted.");
                }
                else
                {
                    Console.WriteLine("Exiting.");
                }
                
            }
            else
            {
                Console.WriteLine("Invalid key pressed.");
            }
            

        }
    }
}
