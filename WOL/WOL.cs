using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
namespace OOPs
{
    class WOL
    {
        private byte[] mac;
        public WOL()
        {
        }

        //MAC Address should  look like '013FA049'
        public bool Wake(string MACAddress)
        {
            Regex regex = new Regex(@"[a-f0-9]{12}$", RegexOptions.IgnoreCase);
            if (regex.IsMatch(MACAddress))
            {
                mac = new byte[6];
                for (int i = 0; i < 6; i++)
                    mac[i] = byte.Parse(MACAddress.Substring(i * 2, 2), NumberStyles.HexNumber);

                return WakeUp();
            }
            else
            {
                return false;
            }
        }

        private bool WakeUp()
        {
            try
            {
                //
                // WOL packet is sent over UDP 255.255.255.255:12345.
                //
                UdpClient client = new UdpClient();
                client.Connect(IPAddress.Broadcast, 12345);

                //
                // WOL packet contains a 6-bytes trailer and 16 times a 6-bytes sequence containing the MAC address.
                //
                byte[] packet = new byte[17 * 6];

                //
                // Trailer of 6 times 0xFF.
                //
                for (int i = 0; i < 6; i++)
                    packet[i] = 0xFF;

                //
                // Body of magic packet contains 16 times the MAC address.
                //
                for (int i = 1; i <= 16; i++)
                    for (int j = 0; j < 6; j++)
                        packet[i * 6 + j] = mac[j];

                //
                // Submit WOL packet.
                //
                client.Send(packet, packet.Length);

                // Ok
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
