﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace DFS.Views
{
    public partial class PaymentPage : ContentPage
    {
        public PaymentPage(string url)
        {
            InitializeComponent();
            webView.Source = url;
            webView.Navigated+=WebView_Navigated;
        }

        async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            if (e.Url.StartsWith("http://104.238.81.169:4080/FitnessApp/manageservices/v1/members/process/payment", StringComparison.CurrentCultureIgnoreCase))
            {
                await DisplayAlert("Alert", "Payment successfull!", "OK");
                App.Current.MainPage = new RootPage(App.SelectedView);

            }

            if (e.Url.StartsWith("http://104.238.81.169:4080/FitnessApp/manageservices/v1/members/cancel/payment", StringComparison.CurrentCultureIgnoreCase))
            {
                await DisplayAlert("Alert", "Payment Failure!", "OK");
                await this.Navigation.PopAsync();

            }
        }


    }
}
