using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TutoriasServer
{
    internal class Host
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Host));

        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(ServiceImplementacion.TutoriaService)))
            {
                
                
                    try
                {
                    host.Open();
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("          - - - - - - - - - - - - - - ");
                    Console.WriteLine("");
                    Console.WriteLine("          Server Azul Game is running");
                    Console.WriteLine("");
                    Console.WriteLine("          - - - - - - - - - - - - - - ");
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("          Presiona [Enter] para detener el servidor.");
                    Console.ReadLine();
                }
                catch (CommunicationException)
                {
                    throw new ServerCommunicationException();
                }
                catch (TimeoutException)
                {
                    throw new ServerTimeoutException();
                }
                catch (Exception)
                {
                    throw new ServerStartupException();
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                        Console.WriteLine("El host ha sido abortado debido a un fallo.");
                    }
                    else
                    {
                        host.Close();
                        Console.WriteLine("El host se ha cerrado correctamente.");
                    }
                }
                
                
            }
        }
    }
}
