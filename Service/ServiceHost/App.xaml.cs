using System;
using System.Threading.Tasks;
using System.Windows;
using log4net;
using log4net.Config;
using ServiceImplementacion;
using System.ServiceModel;

[assembly: XmlConfigurator(Watch = true)]
namespace ServiceHost
{
    public partial class App : Application
    {
        private System.ServiceModel.ServiceHost _host;
        private static readonly ILog logger = LogManager.GetLogger(typeof(App));
        public ServiceHostBase WcfHost { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            XmlConfigurator.Configure();

            Task.Run(() =>
            {
                try
                {
                    _host = new System.ServiceModel.ServiceHost(typeof(TutoringFacadeService));
                    _host.Open();

                    Dispatcher.Invoke(() => MessageBox.Show(
                        "✅ Servicio WCF iniciado en net.tcp://localhost:8095/TutoriaService\n\n" +
                        "  • MEX TCP en net.tcp://localhost:8095/TutoriaService/mex",
                        "Servicio iniciado",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    ));
                }
                catch (Exception ex)
                {
                    logger.Error("Error al iniciar el host WCF", ex);
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(
                            "❌ No pude iniciar el servicio:\n" + ex.Message,
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                        Shutdown();
                    });
                }
            });

            var main = new MainWindow();
            main.Show();
        }

        public void StopWcfHost()
        {
            if (WcfHost == null) return;

            try
            {
                WcfHost.Close();
            }
            catch
            {
                WcfHost.Abort();
            }
            finally
            {
                WcfHost = null;
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            StopWcfHost();
            base.OnExit(e);
        }
    }
}

