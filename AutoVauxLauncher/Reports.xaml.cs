using ARMDatabase;
using AutoVauxLauncher.HelpClasses;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Office.Interop.Word;
using Ookii.Dialogs.Wpf;
using System;
using System.Data;
using System.Data.Entity;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static AutoVauxLauncher.App;
using Word = Microsoft.Office.Interop.Word;
namespace AutoVauxLauncher
{
    /// <summary>
    /// Логика взаимодействия для TestPage.xaml
    /// </summary>
    public partial class Reports : UserControl
    {
        string document_NamePDF="";
        string document_NameDOC="";
        private static readonly string appData = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyLoginSaver");
        private void ShowOpenFileDialog()
        {
            // As of .Net 3.5 SP1, WPF's Microsoft.Win32.OpenFileDialog class still uses the old style
            VistaOpenFileDialog dialog = new VistaOpenFileDialog();
            dialog.Filter = "All files (*.*)|*.*";
            if (!VistaFileDialog.IsVistaFileDialogSupported)
            {
                MessageBoxUI mui = new MessageBoxUI("Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", MessageType.Warning, MessageButtons.Ok);
                mui.ShowDialog();
            }
            // if ((bool)dialog.ShowDialog(this))
            //   MessageBox.Show(this, "The selected file was: " + dialog.FileName, "Sample open file dialog");
        }
        private void SendToPrinter(object sender, RoutedEventArgs e)
        {
            PrintDocument pd = new PrintDocument();
            PrintDialog printDialog = new PrintDialog();
            if (bronDataGrid.SelectedItem != null)
            {
                string filename = (bronDataGrid.SelectedItem as ReportsTable).Filename;
                pd.DocumentName = filename;
                if (printDialog.ShowDialog() == true)
                pd.Print();
            }
            else
            {
                MessageBoxUI mui = new MessageBoxUI("Не выбран файл для отправки на печать.", MessageType.Warning, MessageButtons.Ok);
                mui.ShowDialog();
            }
        }
        AutovauxContext cs;
        public Reports()
        {
            InitializeComponent();
            samples.Items.Add("Расписание");
            samples.Items.Add("Лист бронирования");
            AutovauxContext cs=new AutovauxContext();
        }
        private void SaveAsPDF(object sender, RoutedEventArgs e)
        {
                if (samples.SelectedItem != null)
                {
                    switch (samples.SelectedItem.ToString())
                    {
                        case "Расписание":
                        document_NamePDF = ShowSavePDFFileDialog();
                        if (document_NamePDF != "")
                        {
                            string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), document_NamePDF);
                            File.Delete(filePath);
                            iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document();
                            using (FileStream fs = new FileStream(filePath, FileMode.CreateNew))
                            {
                                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, fs);
                                try
                                {
                                    string tnr = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.TTF");
                                    string tnrbd = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "timesbd.TTF");
                                    BaseFont times = BaseFont.CreateFont(tnr, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                                    BaseFont timesbd = BaseFont.CreateFont(tnrbd, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                                    iTextSharp.text.Font font = new iTextSharp.text.Font(times, 14, iTextSharp.text.Font.NORMAL);
                                    iTextSharp.text.Font fontb = new iTextSharp.text.Font(timesbd, 14, iTextSharp.text.Font.NORMAL);
                                    pdfDoc.Open();
                                    PdfPTable pdfTable = new PdfPTable(4);
                                    iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph("Расписание на дату " + DateTime.Parse(date.Text).ToString("dd-MM-yyyy"), fontb);
                                    p.Alignment = Element.ALIGN_CENTER;
                                    p.SpacingAfter = 8f;
                                    pdfDoc.Add(p);
                                    pdfTable.SetWidths(new float[] { 7f, 3.5f, 3f, 3f });
                                    pdfTable.AddCell(new PdfPCell(new Phrase("Маршрут", fontb)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingBottom = 8f });
                                    pdfTable.AddCell(new PdfPCell(new Phrase("Время отправления", fontb)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingBottom = 8f });
                                    pdfTable.AddCell(new PdfPCell(new Phrase("Время прибытия", fontb)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingBottom = 8f });
                                    pdfTable.AddCell(new PdfPCell(new Phrase("Номер автобуса", fontb)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingBottom = 8f });
                                    DateTime d = DateTime.Parse(date.Text);
                                    var row = (from timetable in cs.TimetableList
                                               join route in cs.Routes on timetable.ROUTE_ID_FK equals route.ROUTE_ID
                                               where timetable.DATE == d
                                               select new Custom
                                               {
                                                   DEPARTURE_TIME = timetable.DEPARTURE_TIME,
                                                   ARRIVAL_TIME = timetable.ARRIVAL_TIME,
                                                   ROUTE = route.ROUTE,
                                                   BUS_ID = route.BUS_ID_FK
                                               }).ToList();
                                    cs.TimetableTemp.Load();
                                    cs.Routes.Load();
                                    cs.Bus_fleet.Load();
                                    foreach (var r in row)
                                    {
                                        pdfTable.AddCell(new PdfPCell(new Phrase(r.ROUTE, font)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingBottom = 8f });
                                        pdfTable.AddCell(new PdfPCell(new Phrase(r.DEPARTURE_TIME.ToString("HH:mm"), font)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingBottom = 8f });
                                        pdfTable.AddCell(new PdfPCell(new Phrase(r.ARRIVAL_TIME.ToString("HH:mm"), font)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingBottom = 8f });
                                        pdfTable.AddCell(new PdfPCell(new Phrase(r.BUS_ID.ToString(), font)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingBottom = 8f });
                                    }
                                    pdfDoc.Add(pdfTable);
                                    pdfDoc.Close();
                                    writer.Close();
                                    MessageBoxUI mui = new MessageBoxUI($"Сохранено в {filePath}", MessageType.Success, MessageButtons.Ok);
                                    mui.ShowDialog();
                                    ReportsTable reportsTable = new ReportsTable()
                                    {
                                        Date = DateTime.Now.Date.ToString("dd.MM.yyyy"),
                                        Filename = filePath
                                    };
                                    reports = reportsTable;
                                    bronDataGrid.Items.Add(reports);
                                }
                                catch { }
                            }
                        }
                            break;
                        case "Лист бронирования":
                        document_NamePDF = ShowSavePDFFileDialog();
                        if (document_NamePDF != "")
                        {
                            string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), document_NamePDF);
                            File.Delete(filePath);
                            iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document();
                            using (FileStream fs = new FileStream(filePath, FileMode.CreateNew))
                            {
                                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, fs);
                                try
                                {
                                    string tnr = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.TTF");
                                    string tnrbd = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "timesbd.TTF");
                                    BaseFont times = BaseFont.CreateFont(tnr, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                                    BaseFont timesbd = BaseFont.CreateFont(tnrbd, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                                    iTextSharp.text.Font font = new iTextSharp.text.Font(times, 12, iTextSharp.text.Font.NORMAL);
                                    iTextSharp.text.Font fontb = new iTextSharp.text.Font(timesbd, 12, iTextSharp.text.Font.NORMAL);
                                    pdfDoc.Open();
                                    PdfPTable pdfTable = new PdfPTable(5);
                                    iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph("Лист бронирования на дату " + DateTime.Now.Date.ToString("dd-MM-yyyy"), fontb);
                                    p.Alignment = Element.ALIGN_CENTER;
                                    p.SpacingAfter = 8f;
                                    pdfDoc.Add(p);
                                    pdfTable.SetWidths(new float[] { 3f, 3f, 3f, 5.5f, 3f });
                                    pdfTable.AddCell(new PdfPCell(new Phrase("Дата", fontb)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingBottom = 8f });
                                    pdfTable.AddCell(new PdfPCell(new Phrase("Номер автобуса", fontb)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingBottom = 8f });
                                    pdfTable.AddCell(new PdfPCell(new Phrase("Время отправления", fontb)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingBottom = 8f });
                                    pdfTable.AddCell(new PdfPCell(new Phrase("Путь", fontb)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingBottom = 8f });
                                    pdfTable.AddCell(new PdfPCell(new Phrase("Кол-во мест", fontb)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingBottom = 8f });
                                    var row = cs.Bron.Select(x => x).ToList();
                                    cs.Bron.Load();
                                    foreach (var r in row)
                                    {
                                        pdfTable.AddCell(new PdfPCell(new Phrase(r.DATE.ToString("dd.MM.yyyy"), font)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingBottom = 8f });
                                        pdfTable.AddCell(new PdfPCell(new Phrase(r.BUS.ToString(), font)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingBottom = 8f });
                                        pdfTable.AddCell(new PdfPCell(new Phrase(r.DEPART_TIME.ToString("HH:mm"), font)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingBottom = 8f });
                                        pdfTable.AddCell(new PdfPCell(new Phrase(r.ONTOUR, font)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingBottom = 8f });
                                        pdfTable.AddCell(new PdfPCell(new Phrase(r.COUNT.ToString(), font)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_CENTER, PaddingBottom = 8f });
                                    }
                                    pdfDoc.Add(pdfTable);
                                    pdfDoc.Close();
                                    writer.Close();
                                    ReportsTable reportsTable = new ReportsTable()
                                    {
                                        Date = DateTime.Now.Date.ToString("dd.MM.yyyy"),
                                        Filename = filePath
                                    };
                                    reports = reportsTable;
                                    bronDataGrid.Items.Add(reports);
                                    MessageBoxUI mui = new MessageBoxUI($"Сохранено в {filePath}", MessageType.Success, MessageButtons.Ok);
                                    mui.ShowDialog();
                                }
                                catch { }
                            }
                        }
                                break;
                                default: 
                                    break;
                    }
                }
        }
        private string ShowSavePDFFileDialog()
        {
            VistaSaveFileDialog dialog = new VistaSaveFileDialog();
            dialog.Filter = "PDF (*.pdf)|*.pdf";
            dialog.DefaultExt = "pdf";
            string f = "";
            // As of .Net 3.5 SP1, WPF's Microsoft.Win32.SaveFileDialog class still uses the old style
            if (!VistaFileDialog.IsVistaFileDialogSupported)
            {
                MessageBoxUI mui = new MessageBoxUI("Используется старый дизайн диалогового окна", MessageType.Warning, MessageButtons.Ok);
                mui.ShowDialog();
            } 
            if ((bool)dialog.ShowDialog())
            {
                f=dialog.FileName;
            }
            return f;
        }
        private string ShowSaveDOCFileDialog()
        {
            VistaSaveFileDialog dialog = new VistaSaveFileDialog();
            dialog.Filter = "DOC (*.doc)|*.doc";
            dialog.DefaultExt = "doc";
            string f = "";
            // As of .Net 3.5 SP1, WPF's Microsoft.Win32.SaveFileDialog class still uses the old style
            if (!VistaFileDialog.IsVistaFileDialogSupported)
            {
                MessageBoxUI mui = new MessageBoxUI("Используется старый дизайн диалогового окна", MessageType.Warning, MessageButtons.Ok);
                mui.ShowDialog();
            }
            if ((bool)dialog.ShowDialog())
            {
                f = dialog.FileName;
            }
            return f;
        }
        private void SaveAsDoc(object sender, RoutedEventArgs e)
        {
            try
            {
                if (samples.SelectedItem != null)
                {
                    switch (samples.SelectedItem.ToString())
                    {
                        case "Расписание":
                            document_NameDOC = ShowSaveDOCFileDialog();
                            if (document_NameDOC != "")
                            {
                                Word.Application word = new Word.Application();
                                // Создаем новый документ Word
                                Word.Document doc = word.Documents.Add();
                                Word.Paragraph para = doc.Content.Paragraphs.Add();
                                para.Range.Text = "Расписание на дату " + DateTime.Parse(date.Text).ToString("dd-MM-yyyy");
                                para.Range.Font.Name = "Times New Roman";
                                para.Range.Font.Size = 14;
                                para.Range.Font.Bold = 1;
                                para.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                Word.Paragraph para2 = doc.Content.Paragraphs.Add();
                                Word.Paragraph para3 = doc.Content.Paragraphs.Add();
                                Word.Table table = doc.Tables.Add(doc.Range(para2.Range.End, para3.Range.Start), 1, 4);
                                table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                                table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                                // Задаем заголовки столбцов таблицы
                                table.Cell(1, 1).Range.Text = "Маршрут";
                                table.Cell(1, 3).Range.Text = "Время прибытия";
                                table.Cell(1, 2).Range.Text = "Время отправления";
                                table.Cell(1, 4).Range.Text = "Номер автобуса";
                                for (int i = 1; i <= 4; i++)
                                {
                                    table.Cell(1, i).Range.Font.Size = 14;
                                    table.Cell(1, i).Range.Font.Name = "Times New Roman";
                                    table.Cell(1, i).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                    table.Cell(1, i).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                                }
                                table.Cell(1, 1).PreferredWidth = word.Application.CentimetersToPoints(7f);
                                table.Cell(1, 2).PreferredWidth = word.Application.CentimetersToPoints(3.5f);
                                table.Cell(1, 3).PreferredWidth = word.Application.CentimetersToPoints(3f);
                                table.Cell(1, 4).PreferredWidth = word.Application.CentimetersToPoints(3f);
                                DateTime d = DateTime.Parse(date.Text);
                                var row = (from timetable in cs.TimetableList
                                           join route in cs.Routes on timetable.ROUTE_ID_FK equals route.ROUTE_ID
                                           where timetable.DATE == d
                                           select new Custom
                                           {
                                               DEPARTURE_TIME = timetable.DEPARTURE_TIME,
                                               ARRIVAL_TIME = timetable.ARRIVAL_TIME,
                                               ROUTE = route.ROUTE,
                                               BUS_ID = route.BUS_ID_FK
                                           }).ToList();
                                cs.TimetableTemp.Load();
                                cs.Routes.Load();
                                cs.Bus_fleet.Load();
                                int rec = 2;
                                foreach (var r in row)
                                {
                                    table.Rows.Add();
                                    table.Cell(rec, 1).Range.Text = r.ROUTE;
                                    table.Cell(rec, 3).Range.Text = r.ARRIVAL_TIME.ToString("HH:mm");
                                    table.Cell(rec, 2).Range.Text = r.DEPARTURE_TIME.ToString("HH:mm");
                                    table.Cell(rec, 4).Range.Text = r.BUS_ID.ToString();
                                    table.Cell(rec, 1).Range.Bold = 0;
                                    table.Cell(rec, 2).Range.Bold = 0;
                                    table.Cell(rec, 3).Range.Bold = 0;
                                    table.Cell(rec, 4).Range.Bold = 0;
                                    rec++;
                                }
                                string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), document_NameDOC);
                                doc.SaveAs2(filePath);
                                ReportsTable reportsTable = new ReportsTable()
                                {
                                    Date = DateTime.Now.Date.ToString("dd.MM.yyyy"),
                                    Filename = filePath
                                };
                                reports = reportsTable;
                                bronDataGrid.Items.Add(reports);
                                MessageBoxUI mui = new MessageBoxUI($"Сохранено в {filePath}", MessageType.Success, MessageButtons.Ok);
                                mui.ShowDialog();
                                doc.Close();
                                word.Quit();
                            }
                            break;
                        case "Лист бронирования":
                            document_NameDOC = ShowSaveDOCFileDialog();
                            if (document_NameDOC != "")
                            {
                                Word.Application word = new Word.Application();
                                // Создаем новый документ Word
                                Word.Document doc = word.Documents.Add();
                                Word.Paragraph para = doc.Content.Paragraphs.Add();
                                para.Range.Text = "Лист бронирования на дату " + DateTime.Now.Date.ToString("dd-MM-yyyy");
                                para.Range.Font.Name = "Times New Roman";
                                para.Range.Font.Size = 14;
                                para.Range.Font.Bold = 1;
                                para.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                Word.Paragraph para2 = doc.Content.Paragraphs.Add();
                                Word.Paragraph para3 = doc.Content.Paragraphs.Add();
                                Word.Table table = doc.Tables.Add(doc.Range(para2.Range.End, para3.Range.Start), 1, 5);
                                table.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                                table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                                // Задаем заголовки столбцов таблицы
                                table.Cell(1, 1).Range.Text = "Дата";
                                table.Cell(1, 2).Range.Text = "Номер автобуса";
                                table.Cell(1, 3).Range.Text = "Время отправления";
                                table.Cell(1, 4).Range.Text = "Путь";
                                table.Cell(1, 5).Range.Text = "Кол-во мест";
                                for (int i = 1; i <= 5; i++)
                                {
                                    table.Cell(1, i).Range.Font.Size = 14;
                                    table.Cell(1, i).Range.Font.Name = "Times New Roman";
                                    table.Cell(1, i).Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                                    table.Cell(1, i).VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                                }
                                table.Cell(1, 1).PreferredWidth = word.Application.CentimetersToPoints(3f);
                                table.Cell(1, 2).PreferredWidth = word.Application.CentimetersToPoints(3f);
                                table.Cell(1, 3).PreferredWidth = word.Application.CentimetersToPoints(3f);
                                table.Cell(1, 4).PreferredWidth = word.Application.CentimetersToPoints(5.5f);
                                table.Cell(1, 5).PreferredWidth = word.Application.CentimetersToPoints(3f);
                                var row = cs.Bron.Select(x => x).ToList();
                                cs.Bron.Load();
                                int rec = 2;
                                foreach (var r in row)
                                {
                                    table.Rows.Add();
                                    table.Cell(rec, 1).Range.Text = r.DATE.ToString("dd.MM.yyyy");
                                    table.Cell(rec, 2).Range.Text = r.BUS.ToString();
                                    table.Cell(rec, 3).Range.Text = r.DEPART_TIME.ToString("HH:mm");
                                    table.Cell(rec, 4).Range.Text = r.ONTOUR;
                                    table.Cell(rec, 5).Range.Text = r.COUNT.ToString();
                                    table.Cell(rec, 1).Range.Bold = 0;
                                    table.Cell(rec, 2).Range.Bold = 0;
                                    table.Cell(rec, 3).Range.Bold = 0;
                                    table.Cell(rec, 4).Range.Bold = 0;
                                    table.Cell(rec, 5).Range.Bold = 0;
                                    rec++;
                                }
                                string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), document_NameDOC);
                                doc.SaveAs2(filePath);
                                ReportsTable reportsTable = new ReportsTable()
                                {
                                    Date = DateTime.Now.Date.ToString("dd.MM.yyyy"),
                                    Filename = filePath
                                };
                                reports = reportsTable;
                                bronDataGrid.Items.Add(reports);
                                MessageBoxUI mui = new MessageBoxUI($"Сохранено в {filePath}", MessageType.Success, MessageButtons.Ok);
                                mui.ShowDialog();
                                doc.Close();
                                word.Quit();
                            }
                            break;
                        default: break;
                    }
                }
        }
            catch
            {
                MessageBoxUI mui = new MessageBoxUI("Библиотека Microsoft.Office.Interop.Word не найдена", MessageType.Error, MessageButtons.Ok);
        mui.ShowDialog();
            }
}
        private void NextClickedFirst(object sender, RoutedEventArgs e)
        {
            if (samples.SelectedItem != null)
            {
                switch (samples.SelectedItem.ToString())
                {
                    case "Расписание":
                        back.Visibility = Visibility.Visible;
                        samples.Visibility = Visibility.Hidden;
                        choosedate.Visibility = Visibility.Visible;
                        date.Visibility = Visibility.Visible;
                        choosesample.Visibility = Visibility.Hidden;
                        if (date.Value!=null)
                        {
                            choosedate.Visibility = Visibility.Hidden;
                            date.Visibility = Visibility.Hidden;
                            spdf.Visibility = Visibility.Visible;
                            sdoc.Visibility = Visibility.Visible;
                            saveas.Visibility = Visibility.Visible;
                            back.Visibility = Visibility.Hidden;
                            back2.Visibility = Visibility.Visible;
                            next.Visibility = Visibility.Hidden;
                        }
                        break;
                    case "Лист бронирования":
                        back.Visibility = Visibility.Visible;
                        next.Visibility = Visibility.Hidden;
                        spdf.Visibility = Visibility.Visible;
                        sdoc.Visibility = Visibility.Visible;
                        saveas.Visibility = Visibility.Visible;
                        choosesample.Visibility = Visibility.Hidden;
                        samples.Visibility = Visibility.Hidden;
                        break;
                    default: break;
                }
            }
        }
        private void UCLoaded(object sender, RoutedEventArgs e)
        {
            cs = new AutovauxContext();
            if (bronDataGrid.Items != null)
            {
                bronDataGrid.Visibility = Visibility.Visible;
            }
        }
        private void UCUnloaded(object sender, RoutedEventArgs e)
        {
            cs.Dispose();
        }
        private void BackClickedFirst(object sender, RoutedEventArgs e)
        {
            samples.Visibility = Visibility.Visible;
            choosesample.Visibility = Visibility.Visible;
            samples.SelectedItem = null;
            spdf.Visibility = Visibility.Hidden;
            sdoc.Visibility = Visibility.Hidden;
            choosedate.Visibility = Visibility.Hidden;
            date.Visibility = Visibility.Hidden;
            back.Visibility = Visibility.Hidden;
            next.Visibility = Visibility.Visible;
            saveas.Visibility = Visibility.Hidden;
        }
        private void BackClickedSecond(object sender, RoutedEventArgs e)
        {
            date.Value=null;
            spdf.Visibility = Visibility.Hidden;
            sdoc.Visibility = Visibility.Hidden;
            choosedate.Visibility = Visibility.Visible;
            date.Visibility = Visibility.Visible;
            back.Visibility = Visibility.Visible;
            back2.Visibility = Visibility.Hidden;
            next.Visibility = Visibility.Visible;
            saveas.Visibility = Visibility.Hidden;
        }
        private void RepsClicked(object sender, RoutedEventArgs e)
        {
            bronDataGrid.Visibility = Visibility.Visible;
        }
        private void MasterClicked(object sender, RoutedEventArgs e)
        {
            bronDataGrid.Visibility = Visibility.Hidden;
        }
    }
}
