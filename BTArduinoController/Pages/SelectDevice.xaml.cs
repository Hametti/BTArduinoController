using Android.Bluetooth;
using BTArduinoController.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BTArduinoController.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectDevice : ContentPage
    {
        public SelectDevice()
        {
            InitializeComponent();
            DisplayBondedDevices();
        }

        protected override void OnAppearing()
        {
            BondedDevices.SelectedItem = null;
        }

        private void DisplayBondedDevices()
        {
            try
            {
                var adapter = BluetoothAdapter.DefaultAdapter;

                if (adapter == null)
                    CallAlert("Adapter error", "BluetoothAdapter.DefaultAdapter is null", "Ok");

                if (!adapter.IsEnabled)
                    CallAlert("Adapter error", "Bluetooth is not enabled. Enable bluetooth.", "Ok");
                else BondedDevices.ItemsSource = adapter.BondedDevices;
            }
            catch (Exception e)
            {
                CallAlert("Error", $"Something went wrong, exception message: {e.Message}", "Ok");
            }
        }

        private async void BondedDevices_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var device = (BluetoothDevice)e.SelectedItem;

            if (device != null)
            {
                CurrentDevice.Name = device.Name;
                CurrentDevice.Address = device.Address;
                await Navigation.PushAsync(new DevicePanel());
            }
        }

        private async void CallAlert(string title, string message, string cancel) => await DisplayAlert(title, message, cancel);
    }
}