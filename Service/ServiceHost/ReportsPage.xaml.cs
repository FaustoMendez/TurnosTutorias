using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using PdfSharp.Pdf;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceHost.Callbacks;
using ServiceHost.Properties;
using System.IO;

namespace ServiceHost
{
    public partial class ReportsPage : Page
    {
        private readonly ITutoringService tutoringService;
        private readonly IUserService userService;

        public ObservableCollection<ReportItem> ReportItems { get; }
            = new ObservableCollection<ReportItem>();

        public ReportsPage()
        {
            InitializeComponent();
            DataContext = this;
            reportGrid.ItemsSource = ReportItems;
            btnPrint.Visibility = Visibility.Collapsed;

            var cb = new NotificationCallbackHandler();
            var ctx = new InstanceContext(cb);
            tutoringService = new DuplexChannelFactory<ITutoringService>(
                ctx, "NetTcpBinding_ITutoringService")
                .CreateChannel();
            userService = new ChannelFactory<IUserService>(
                "NetTcpBinding_IUserService")
                .CreateChannel();
        }

        private void OnDateChanged(object sender, SelectionChangedEventArgs e)
        {
            btnGenerate.IsEnabled =
                dpFrom.SelectedDate.HasValue &&
                dpTo.SelectedDate.HasValue &&
                dpFrom.SelectedDate <= dpTo.SelectedDate;
        }

        [DebuggerNonUserCode]
        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            if (!dpFrom.SelectedDate.HasValue || !dpTo.SelectedDate.HasValue)
            {
                MessageBox.Show(
                    Properties.Resources.PleaseSelectBothDates,
                    Properties.Resources.ValidationErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var fromDate = dpFrom.SelectedDate.Value;
            var toDate = dpTo.SelectedDate.Value;
            if (fromDate > toDate)
            {
                MessageBox.Show(
                    Properties.Resources.StartDateBeforeEndError,
                    Properties.Resources.ValidationErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var from = fromDate;
            var to = toDate.Date.AddDays(1).AddTicks(-1);

            ObservableCollection<ReportItem> temp = new ObservableCollection<ReportItem>();

            try
            {
                var appointments = tutoringService
                    .GetAttendedAppointment(from, to)
                    .ToList();

                if (appointments.Count == 0)
                {
                    MessageBox.Show(
                        Properties.Resources.NoAppointmentsFound,
                        Properties.Resources.InfoTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    reportGrid.ItemsSource = null;
                    btnPrint.Visibility = Visibility.Collapsed;
                    return;
                }

                var students = userService
                    .GetAllUsers()
                    .ToDictionary(
                        u => u.UserId,
                        u => $"{u.FirstName} {u.PaternalSurname} {u.MaternalSurname}"
                    );

                var tutors = userService
                    .GetAvailableTutors()
                    .ToDictionary(
                        t => t.TutorId,
                        t => $"{t.FirstName} {t.PaternalSurname} {t.MaternalSurname}"
                    );

                foreach (var r in appointments)
                {
                    temp.Add(new ReportItem
                    {
                        TurnoId = r.AppointmentId,
                        Matricula = r.StudentId,
                        Estudiante = students.TryGetValue(r.StudentId, out var en) ? en : r.StudentName,
                        Tutor = tutors.TryGetValue(r.TutorId, out var tn) ? tn : r.TutorId,
                        Fecha = r.SessionDate.Date
                    });
                }

                ReportItems.Clear();
                foreach (var item in temp)
                    ReportItems.Add(item);

                reportGrid.ItemsSource = ReportItems;

                if (ReportItems.Count > 0)
                {
                    btnPrint.Visibility = Visibility.Visible;
                    btnPrint.IsEnabled = true;
                }
                else
                {
                    btnPrint.Visibility = Visibility.Collapsed;
                }
            }
            catch (FaultException fe)
            {
                MessageBox.Show(
                    fe.Message,
                    Properties.Resources.ValidationErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(Properties.Resources.UnexpectedErrorMessage, ex.Message),
                    Properties.Resources.ErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        [DebuggerNonUserCode]
        private void PrintReport_Click(object sender, RoutedEventArgs e)
        {
            if (ReportItems == null || ReportItems.Count == 0)
                return;

            try
            {
                var pdfPath = CreatePdfReport(ReportItems);
                Process.Start(new ProcessStartInfo
                {
                    FileName = pdfPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(Properties.Resources.PdfGenerationError, ex.Message),
                    Properties.Resources.ErrorTitle,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private string CreatePdfReport(ObservableCollection<ReportItem> items)
        {
            var doc = new Document();
            var sec = doc.AddSection();
            sec.AddParagraph(Properties.Resources.ReportsHeader, "Heading1");

            var table = sec.AddTable();
            table.Format.Alignment = ParagraphAlignment.Center;
            table.Borders.Width = 0.75;
            foreach (var w in new[] { "2cm", "3cm", "4cm", "4cm", "3cm" })
                table.AddColumn(w);

            var header = table.AddRow();
            header.Shading.Color = Colors.LightGray;
            header.Cells[0].AddParagraph(Properties.Resources.HeaderShiftId);
            header.Cells[1].AddParagraph(Properties.Resources.HeaderMatricula);
            header.Cells[2].AddParagraph(Properties.Resources.HeaderStudent);
            header.Cells[3].AddParagraph(Properties.Resources.HeaderTutor);
            header.Cells[4].AddParagraph(Properties.Resources.HeaderDate);

            foreach (var it in items)
            {
                var row = table.AddRow();
                row.Cells[0].AddParagraph(it.TurnoId.ToString());
                row.Cells[1].AddParagraph(it.Matricula);
                row.Cells[2].AddParagraph(it.Estudiante);
                row.Cells[3].AddParagraph(it.Tutor);
                row.Cells[4].AddParagraph(it.Fecha.ToString("d"));
            }

            #pragma warning disable CS0618
            var renderer = new PdfDocumentRenderer(true) { Document = doc };
            #pragma warning restore CS0618
            renderer.RenderDocument();

            var temp = Path.Combine(
                Path.GetTempPath(),
                $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            renderer.PdfDocument.Save(temp);
            return temp;
        }

        public class ReportItem
        {
            public int TurnoId { get; set; }
            public string Matricula { get; set; }
            public string Estudiante { get; set; }
            public string Tutor { get; set; }
            public DateTime Fecha { get; set; }
        }
    }
}



