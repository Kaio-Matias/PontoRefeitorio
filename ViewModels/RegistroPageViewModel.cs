using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PontoRefeitorio.ViewModels
{
    public partial class RegistroPageViewModel : ObservableObject
    {
        [ObservableProperty] private string horaAtual;
        [ObservableProperty] private string dataAtual;
        private Timer _timer;

        public RegistroPageViewModel()
        {
            UpdateDateTime(null);
            _timer = new Timer(UpdateDateTime, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void UpdateDateTime(object state)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                HoraAtual = DateTime.Now.ToString("HH:mm:ss");
                DataAtual = DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy");
            });
        }
    }
}