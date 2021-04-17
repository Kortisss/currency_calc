using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using System.Xml.Linq;
using Windows.Storage;
using System.Text.RegularExpressions;
using System.Drawing;
using Windows.UI;
using System.Net;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace kalkulator_walut___Michal_Pieczka
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const string daneNBP = "http://www.nbp.pl/kursy/xml/LastA.xml"; //tu adres pliku z danymi kursowymi 
        List<PozycjaTabeliA> kursyAktualne = new List<PozycjaTabeliA>(); //lista pozycji z danymi dla kolejnych walut 
        List<DataTabela> dataDokumentu = new List<DataTabela>();

        PozycjaTabeliA zWaluty;
        PozycjaTabeliA naWalute;
        public MainPage()
        {
            this.InitializeComponent();
        }
        public class PozycjaTabeliA
        {
            public string przelicznik { get; set; }
            public string kod_waluty { get; set; }
            public string kurs_sredni { get; set; }


        }

        public class DataTabela
        {
            public string numer_tabeli { get; set; }
            public string data_publikacji { get; set; }
        }

        async private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            var serwerNBP = new HttpClient();
            string dane = "";
            try
            {
                dane = await serwerNBP.GetStringAsync(new Uri(daneNBP));
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
            if (dane != "")
            {
                XDocument daneKursowe = XDocument.Parse(dane);

                kursyAktualne = (
                    from item in daneKursowe.Descendants("pozycja")
                    select new PozycjaTabeliA()
                    {
                        kod_waluty = item.Element("kod_waluty").Value,
                        kurs_sredni = item.Element("kurs_sredni").Value,
                        przelicznik = item.Element("przelicznik").Value
                    }).ToList();

                dataDokumentu = (
                    from item in daneKursowe.Descendants("tabela_kursow")
                    select new DataTabela()
                    {
                        numer_tabeli = item.Element("numer_tabeli").Value,
                        data_publikacji = item.Element("data_publikacji").Value
                    }).ToList();

                kursyAktualne[kursyAktualne.Count-1] = new PozycjaTabeliA()
                {
                    kurs_sredni = "1,0000",
                    kod_waluty = "PLN",
                    przelicznik = "1"
                };

                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("lbxZWaluty"))
                {
                    zWaluty = kursyAktualne[localSettings.Values["lbxZWaluty"].GetHashCode()];
                    kursyAktualne[0] = new PozycjaTabeliA()
                    {
                        kurs_sredni = zWaluty.kurs_sredni,
                        kod_waluty = zWaluty.kod_waluty,
                        przelicznik = zWaluty.przelicznik
                    };
                }

                lbxZWaluty.ItemsSource = kursyAktualne;
                lbxNaWalute.ItemsSource = kursyAktualne;

                DataTabela dataTabela = dataDokumentu[0];
                txtData.Text = dataTabela.data_publikacji;
            }
        }

        private void txtKwota_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (lbxZWaluty.SelectedIndex != -1 || lbxNaWalute.SelectedIndex != -1)
            {
                zWaluty = kursyAktualne[lbxZWaluty.SelectedIndex];
                naWalute = kursyAktualne[lbxNaWalute.SelectedIndex];
                double kwotaPLN;
                double kwotaDocelowa;

                txtWybraneWaluty.Text = String.Format("z {0} -> na {1}", zWaluty.kod_waluty, naWalute.kod_waluty);

                if ((new Regex(@"^[0-9]+(\.[0-9]{1,2})?$")).IsMatch(txtKwota.Text))
                {
                    txtKwota.Foreground = new SolidColorBrush(Colors.Green);

                    kwotaPLN = long.Parse(txtKwota.Text) * Convert.ToDouble(zWaluty.kurs_sredni);
                    kwotaDocelowa = kwotaPLN / Convert.ToDouble(naWalute.kurs_sredni);

                    //string s = string.Format("{0:N2}%", k);
                    tbPrzeliczona.Text = (Math.Truncate(kwotaDocelowa * 100) / 100).ToString().Replace(",", ".");
                }
                else
                {
                    txtKwota.Foreground = new SolidColorBrush(Colors.Red);
                    tbPrzeliczona.Text = " ";
                }
            }
            else
            {
                tbPrzeliczona.Text = "zaznacz obie waluty";
            }


        }

        private void lbxZWaluty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["lbxZWaluty"] = lbxZWaluty.SelectedIndex;
        }

        private void btnOProgramie_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(OProgramie));
        }

        private void btnPomoc_Click(object sender, RoutedEventArgs e)
        {
            if (lbxZWaluty.SelectedIndex != -1)
            {
                var p = new PozycjaTabeliA();
                p = kursyAktualne[lbxZWaluty.SelectedIndex];

                this.Frame.Navigate(typeof(Pomoc), p);
            }
            else
            {
                tbPrzeliczona.Text = "aby przejść do strony 'pomoc ' wybierz walute z lewej listy";
            }
        }
    }
}
