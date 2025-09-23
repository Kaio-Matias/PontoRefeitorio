// Arquivo: ViewModels/RegistroPageViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace PontoRefeitorio.ViewModels
{
    public partial class RegistroPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _horaAtual;

        [ObservableProperty]
        private string _dataAtual;

        private Timer _timer;

        public RegistroPageViewModel()
        {
            AtualizarHorario(null); // Atualiza o horário assim que a página é criada
            // Inicia um timer para atualizar o horário a cada segundo
            _timer = new Timer(AtualizarHorario, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void AtualizarHorario(object state)
        {
            // Formata a hora e a data conforme o padrão brasileiro
            HoraAtual = DateTime.Now.ToString("HH:mm:ss");
            DataAtual = DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy");
        }
    }
}