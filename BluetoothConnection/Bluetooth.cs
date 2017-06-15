using System;
using System.Collections.Generic;
#if NETFX_CORE
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif
namespace BluetoothConnection
{
    public class Bluetooth
    {
#if NETFX_CORE
        private RfcommDeviceService DeviceService;
        private StreamSocket BtSocket;
        private DataWriter Writer;
        private DataReader Reader;
#endif

        public Bluetooth()
        {
        }

        private string _Message = null;
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                if (_Message != value)
                {
                    _Message = value;
                }
            }
        }

        private bool _IsOpen = false;
        public bool IsOpen
        {
            get
            {
                return _IsOpen;
            }
            set
            {
                if (_IsOpen != value)
                {
                    _IsOpen = value;
                }
            }
        }

        public void Open(string deviceName)
        {
#if NETFX_CORE
            if (IsOpen)
            {
                Message = "Already port opened.";
            }

            if (deviceName != null)
            {
                //Bluetoothデバイス名と一致するデバイス情報を取得し接続する 
                var forwards = GuessDeviceNames();
                foreach (var forward in forwards)
                {
                    if (forward == deviceName)
                    {
                        Open(forward);
                        break;
                    }
                }
            }
#endif
        }

#if NETFX_CORE
        private void Open(DeviceInformation serviceInfo)
        {
            try
            {
                //指定されたデバイス情報で接続を行う 
                if (DeviceService == null)
                {
                    DeviceService = RfcommDeviceService.FromIdAsync(serviceInfo.Id).GetResults();
                    BtSocket = new StreamSocket();
                    BtSocket.ConnectAsync(
                        this.DeviceService.ConnectionHostName,
                        this.DeviceService.ConnectionServiceName,
                        SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
                    Writer = new DataWriter(BtSocket.OutputStream);
                    Reader = new DataReader(BtSocket.InputStream);
                    this.Message = "Connected " + DeviceService.ConnectionHostName.DisplayName;
                    IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                this.Message = ex.Message;
                DeviceService = null;
                IsOpen = false;
            }
        }
#endif

        public void Close()
        {
#if NETFX_CORE
            try
            {
                DeviceService = null;
                BtSocket.CancelIOAsync();
                Writer = null;
                Reader = null;
                Message = "Closed";
                IsOpen = false;                
            }
            catch(Exception ex)
            {
                this.Message = ex.Message;
                DeviceService = null;
                IsOpen = false;
            }
#endif
        }

        public void WriteLine(string data)
        {
#if NETFX_CORE
            try
            {
                if (DeviceService != null)
                {
                    Writer.WriteString(data);
                    var sendResult = Writer.StoreAsync();
                }
            }
            catch (Exception ex)
            {
                this.Message = ex.Message;
                DeviceService = null;
                IsOpen = false;
            }
#endif
        }

        public string ReadLine()
        {
#if NETFX_CORE
            try
            {
                if (DeviceService != null)
                {
                    Reader.LoadAsync(512);
                    return Reader.ReadString(32);
                }
                return "";
            }
            catch(Exception ex)
            {
                this.Message = ex.Message;
                DeviceService = null;
                IsOpen = false;
                return "";
            }
#else
            return "";
#endif
        }

        public List<string> GuessDeviceNames()
        {
#if NETFX_CORE
            List<string> deviceNames = new List<string>();
            var serviceInfos = DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort)).GetResults();
            foreach (var serviceInfo in serviceInfos)
            {
                deviceNames.Add(serviceInfo.Name);
            }
            return deviceNames;
#else
            return null;
#endif
        }

    }
}
