using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static kalkulator_walut___Michal_Pieczka.MainPage;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace kalkulator_walut___Michal_Pieczka
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class Pomoc : Page
    {
        public Pomoc()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
                base.OnNavigatedTo(e);
                var parameters = (PozycjaTabeliA)e.Parameter;
                txtPomoc.Text = String.Format("\nInformacje o wybranej walucie:\n\n\n kod waluty: {0} \n kurs średni: {1} \n przelicznik: {2}", parameters.kod_waluty, parameters.kurs_sredni, parameters.przelicznik);


            //if (e.Parameter is string && !string.IsNullOrWhiteSpace((string)e.Parameter))
            //{ txtPomoc.Text += $"Wybrana waluta:  {e.Parameter.ToString()} "; }
        }

            private void btnWroc_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
