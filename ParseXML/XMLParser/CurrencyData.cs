using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLParser
{
    class CurrencyData
    {
        public string date;
        public string unit;
        public string currencyName;
        public string forexBuying;
        public string forexSelling;
        public string banknoteBuying;
        public string banknoteSelling;
        public string crossRateUSD;

        public CurrencyData(string d,string u, string cn, string fb, string fs, string bb, string bs, string cru)
        {
            if(string.IsNullOrEmpty(d))
            {
                date = "null";
            }
            if (string.IsNullOrEmpty(fb))
            {
                fb = "null";
            }
            if (string.IsNullOrEmpty(fs))
            {
                fs = "null";
            }
            if (string.IsNullOrEmpty(bb))
            {
                bb = "null";
            }
            if (string.IsNullOrEmpty(bs))
            {
                bs = "null";
            }
            if(string.IsNullOrEmpty(cru))
            {
                cru = "null";
            }

            date = d;
            unit = u;
            currencyName = cn;
            forexBuying = fb;
            forexSelling = fs;
            banknoteBuying = bb;
            banknoteSelling = bs;
            crossRateUSD = cru;
        }

        public void PrintCurrencyInfo()
        {
            Console.WriteLine("Date: " + date +
                "\nCurrency Name: " + currencyName + 
                "\nUnit: " + unit + 
                "\nForex Buying: " + forexBuying + 
                "\nForex Selling: " + forexSelling + 
                "\nBanknote Buying: " + banknoteBuying + 
                "\nBanknote Selling: " + banknoteSelling +
                "\nCross Rate USD: " + crossRateUSD + "\n");
        }

    }
}
