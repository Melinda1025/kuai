using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Printing;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
using System.Windows.Xps.Serialization;

namespace SDBS3000.ViewModels.Dialogs
{
    public class PrintModel
    {
        public string RotorName { get; set; }
        public double Speed { get; set; }
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public double R1 { get; set; }
        public double R2 { get; set; }
        public double Panel1MaxValue { get; set; }
        public double Panel2MaxValue { get; set; }
        public double StaticMaxValue { get; set; }
        public double Panel1Value { get; set; }
        public double Panel2Value { get; set; }
        public double StaticValue { get; set; }
    }
    public partial class PrintingViewModel : ObservableObject
    {
        private static PrintTicket PrintTicket;
        private static PrintQueue PrintQueue;


        [ObservableProperty]
        private FlowDocument document;

        public PrintModel PrintModel
        {
            get => Document?.DataContext as PrintModel;
            set
            {
                if (Document != null) Document.DataContext = value;
            }
        }

        public PrintingViewModel()
        {
            try
            {
                Document = App.LoadComponent(new Uri("Views/Printing/PrintDocument.xaml", UriKind.Relative)) as FlowDocument;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Document = null;
            }
        }

        [RelayCommand]
        public async Task PrintAsync()
        {
            if (Document == null) return;

            var dialog = new PrintDialog();
            if (PrintTicket != null) dialog.PrintTicket = PrintTicket;
            if (PrintQueue != null) dialog.PrintQueue = PrintQueue;

            if (dialog.ShowDialog() != true) return;

            PrintTicket = dialog.PrintTicket;
            PrintQueue = dialog.PrintQueue;

            var tcs = new TaskCompletionSource();
            var writer = PrintQueue.CreateXpsDocumentWriter(dialog.PrintQueue);
            writer.WritingCompleted += WritingCompleted;
            writer.WritingCancelled += WritingCancelled;
            writer.WritingPrintTicketRequired += WritingPrintTicketRequired;
            try
            {
                writer.WriteAsync(((IDocumentPaginatorSource)Document).DocumentPaginator, dialog.PrintTicket);
                await tcs.Task;
            }
            finally
            {
                writer.WritingCompleted -= WritingCompleted;
                writer.WritingCancelled -= WritingCancelled;
                writer.WritingPrintTicketRequired -= WritingPrintTicketRequired;
            }

            void WritingPrintTicketRequired(object sender, WritingPrintTicketRequiredEventArgs e)
            {
                if (e.CurrentPrintTicketLevel == PrintTicketLevel.FixedDocumentSequencePrintTicket)
                {
                    e.CurrentPrintTicket = dialog.PrintTicket;
                }
            }

            void WritingCompleted(object sender, WritingCompletedEventArgs e)
            {
                if (e.Cancelled)
                {
                    tcs.TrySetCanceled();
                }
                else if (e.Error != null)
                {
                    tcs.TrySetException(e.Error);
                }
                else
                {
                    tcs.TrySetResult();
                }
            }

            void WritingCancelled(object sender, WritingCancelledEventArgs e)
            {
                tcs.TrySetCanceled();
            }
        }
    }
}
