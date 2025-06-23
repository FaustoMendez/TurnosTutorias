using System;
using System.Windows;
using log4net;
using log4net.Config;
using ServiceImplementacion;
using ServiceImplementacion.Services;

[assembly: XmlConfigurator(Watch = true)]
namespace ServiceHost
{
    public partial class App : Application
    {
        private System.ServiceModel.ServiceHost _host;
        private static readonly ILog logger = LogManager.GetLogger(typeof(App));

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            XmlConfigurator.Configure(); // si usas log4net

            try
            {
                _host = new System.ServiceModel.ServiceHost(typeof(TutoringFacadeService));
                _host.Open();
                MessageBox.Show(
                  "✅ Servicio WCF iniciado en net.tcp://localhost:8090/TutoriaService\n\n" +
                  "  • MEX TCP en net.tcp://localhost:8090/TutoriaService/mex",
                  "Servicio iniciado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                logger.Error("Error al iniciar el host WCF", ex);
                MessageBox.Show("❌ No pude iniciar el servicio:\n" + ex.Message,
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_host != null)
            {
                try { _host.Close(); }
                catch { _host.Abort(); }
            }
            base.OnExit(e);
        }
    }
}
