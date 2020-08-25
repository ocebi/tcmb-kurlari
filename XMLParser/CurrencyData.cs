using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLParser
{
    class CurrencyData
    {
        public string unit;
        public string currencyName;
        public string forexBuying;
        public string forexSelling;
        public string banknoteBuying;
        public string banknoteSelling;

        public CurrencyData(string u, string cn, string fb, string fs, string bb, string bs)
        {
            unit = u;
            currencyName = cn;
            forexBuying = fb;
            forexSelling = fs;
            banknoteBuying = bb;
            banknoteSelling = bs;
        }

        public void PrintCurrencyInfo()
        {
            Console.WriteLine("Currency Name: " + currencyName + 
                "\nUnit: " + unit + 
                "\nForex Buying: " + forexBuying + 
                "\nForex Selling: " + forexSelling + 
                "\nBanknote Buying: " + banknoteBuying + 
                "\nBanknote Selling: " + banknoteSelling + "\n");
        }

    }
}
