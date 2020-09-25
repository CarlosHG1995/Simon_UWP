using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ET4_6_Simon
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        const int sequenceTime = 750;
        protected const int flashduration = 250;
        const double luminosidadOff = 0.4;
        const double luminosidadOn = 0.75;
        BoxView[] boxview;
        Color[] colores = { Color.Red, Color.Blue, Color.Green, Color.Yellow };
        List<int> secuencia = new List<int>();
        int indexSec;
        bool esperandoTap;
        bool juegoTerminado;
        Random aleatorio = new Random();
        
        public MainPage()
        {
           
            InitializeComponent();
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += TapGestureRecognizer_Tapped;
            bView1.GestureRecognizers.Add(tap);
            boxview = new BoxView[] { bView0,bView1, bView2, bView3 };
            InicializarColores();
        }

        private void InicializarColores() 
        {
            int indice = 0;
            foreach (var b in boxview) {
                b.Color = colores[indice++].WithLuminosity(luminosidadOff);
                b.BackgroundColor = Color.FromRgba(0.5,0.6,0.9,0);
            }
        }
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (juegoTerminado) return;
            if (!esperandoTap) {
                FinJuego();
                return;
            }
            BoxView pulsado = sender as BoxView;
            int index = Array.IndexOf(boxview,pulsado);
            if (index != secuencia[indexSec]) {
                FinJuego();
                return;
            }
            FlashBoxView(index);
            indexSec++;
            esperandoTap = (indexSec < secuencia.Count);
            if (!esperandoTap) {
                StarSeceuncia();
            }
        }

        private void Empezar_Clicked(object sender, EventArgs e)
        {
            juegoTerminado = false;
            Empezar.IsVisible = false;
            InicializarColores();
            secuencia.Clear();
            StarSeceuncia();
        }

        private void StarSeceuncia()
        {
            secuencia.Add(aleatorio.Next(4));
            indexSec = 0;
            Device.StartTimer(TimeSpan.FromMilliseconds(sequenceTime), OnTimerTick);
        }

        private bool OnTimerTick()
        {
            if (juegoTerminado) return false;
            FlashBoxView(secuencia[indexSec]);
            indexSec++;
            esperandoTap = indexSec == secuencia.Count;
            indexSec = esperandoTap ? 0 : indexSec;
            return !esperandoTap;
        }

        private void FlashBoxView(int v)
        {
            boxview[v].Color = colores[v].WithLuminosity(luminosidadOn);
            Device.StartTimer(TimeSpan.FromMilliseconds(flashduration),
                () => {
                    if (juegoTerminado) return false;
                    boxview[v].Color = colores[v].WithLuminosity(luminosidadOff);
                    return false;
                });
        }

        void FinJuego() {
            juegoTerminado = true;
            for (int i = 0; i < 4; i++) {
                boxview[i].Color = Color.Gray;
            }
            var toastmsm = new ToastConfig("Perdiste");
            toastmsm.SetDuration(3000);
            toastmsm.SetBackgroundColor(System.Drawing.Color.Orange);
            toastmsm.SetMessageTextColor(System.Drawing.Color.White);
            toastmsm.SetPosition(ToastPosition.Bottom);
            UserDialogs.Instance.Toast(toastmsm);
            Empezar.Text = "Intentar de nuevo";
            Empezar.IsVisible = true;
        }
    }
}
