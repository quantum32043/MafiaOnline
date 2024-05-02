using System.Net;
using System.Net.Sockets;
using MafiaOnline.Network;

namespace MafiaOnline
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            Host host = new Host(9850);
            host.Start();

        }

        private void OnCounterClicked2(object sender, EventArgs e) 
        {
            Client client = new Client();
            client.Join();
        }
    }

}
