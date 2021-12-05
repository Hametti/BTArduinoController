using Android.Bluetooth;
using BTArduinoController.Data;
using Java.Util;
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
    public partial class DevicePanel : ContentPage
    {
        private BluetoothDevice _device;
        private BluetoothSocket _socket;

        public DevicePanel()
        {
            InitializeComponent();
            Initialize();
        }
        protected override void OnDisappearing()
        {
            Disconnect();
        }

        void ConnectingMessageState(bool state)
        {
            var connectingMessage = ((Label)FindByName("ConnectingMessage")).IsVisible = state;
        }

        private async void Initialize()
        {
            ConnectingMessageState(true);
            MakeButtonsInvisible();
            try
            {
                var device = BluetoothAdapter.DefaultAdapter.BondedDevices.FirstOrDefault(d => d.Address == CurrentDevice.Address);
                _device = device;
                _socket = _device.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                await _socket.ConnectAsync();
                await DisplayAlert("Connection", "Connection established", "Ok");
                ButtonsConnectedState();
            }
            catch (Exception e)
            {
                CallAlert("Error", $"Can't connect. Make sure bluetooth device is nearby.", "Ok");
                ButtonsDisconnectedState();
            }
            finally
            {
                ConnectingMessageState(false);
            }
        }

        private void MakeButtonsInvisible()
        {
            var connectButton = ((Button)FindByName("btnConnect")).IsVisible = false;
            var disonnectButton = ((Button)FindByName("btnDisconnect")).IsVisible = false;
        }

        void ButtonsConnectedState()
        {
            var connectButton = ((Button)FindByName("btnConnect")).IsVisible = false;
            var disonnectButton = ((Button)FindByName("btnDisconnect")).IsVisible = true;
        }

        void ButtonsDisconnectedState()
        {
            var connectButton = ((Button)FindByName("btnConnect")).IsVisible = true;
            var disonnectButton = ((Button)FindByName("btnDisconnect")).IsVisible = false;
        }


        private async void CallAlert(string title, string message, string cancel) => await DisplayAlert(title, message, cancel);
        private async void btnSendA_Clicked(object sender, EventArgs e)
        {
            if(!_socket.IsConnected)
            {
                await DisplayAlert("Connection", "Your socket is disconnected. Try to connect first", "Ok");
                return;
            }

            byte[] buffer = Encoding.ASCII.GetBytes("a");
            await _socket.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        private async void btnSendB_Clicked(object sender, EventArgs e)
        {
            if (!_socket.IsConnected)
            {
                await DisplayAlert("Connection", "Your socket is disconnected. Try to connect first", "Ok");
                return;
            }

            byte[] buffer = Encoding.ASCII.GetBytes("b");
            await _socket.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        private void btnConnect_Clicked(object sender, EventArgs e)
        {
            if (!_socket.IsConnected)
            {
                _socket = _device.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                Connect();
            }
            else
                CallAlert("Connection", "Bluetooth socket is connected", "Ok");
        }

        private void btnDisconnect_Clicked(object sender, EventArgs e)
        {
            if (_socket.IsConnected)
                Disconnect();
            else
                CallAlert("Connection", "Bluetooth socket is disconnected", "Ok");
        }
        private async void Connect()
        {
            ConnectingMessageState(true);
            ButtonsConnectedState();
            try
            {
                await _socket.ConnectAsync();
                CallAlert("Connection", "Connection established", "Ok");
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", $"Can't connect. Make sure bluetooth device is nearby.", "Ok");
                ButtonsDisconnectedState();
            }
            finally
            {
                ConnectingMessageState(false);
            }
        }
        private async void Disconnect()
        {
            ButtonsDisconnectedState();
            try
            {
                _socket.Close();
                await DisplayAlert("Connection", "Connection closed", "Ok");
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", $"Disconnection error - {e.Message}", "Ok");
                ButtonsConnectedState();
            }
        }

        private void btnCheckStatus_Clicked(object sender, EventArgs e)
        {
            var deviceName = _device.Name;
            var deviceAddress = _device.Address;
            var socketName = _socket.ToString();
            var connectionState = "";

            if (deviceName == null)
                deviceName = "Unknown";

            if (deviceAddress == null)
                deviceAddress = "Unknown";

            if (socketName == null)
                socketName = "Unknown(Connect first)";

            if (_socket.IsConnected == true)
                connectionState = "Connected";
            else
                connectionState = "Not connected";

            CallAlert("Connection status",
                $"Device name: {deviceName} \nMAC address: {deviceAddress} \n\nYour socket: {socketName} \n\nConnection status: \n{connectionState}",
                "Ok");
        }

    }
}