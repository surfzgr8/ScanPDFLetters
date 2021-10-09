using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ScanPDFLetters.Helper;
using ScanPDFLetters.Worker;
using System.Collections.Generic;
using System.Text;
using ScanPDFLetters.Model;

namespace ScanPDFLetters
{
    public partial class frmMain : Form
    {
        ICSVAdapter csvAdapter;
        IValidationReporter validationReporter;
        public frmMain()
        {
            InitializeComponent();
            // Testing code
            if (Debugger.IsAttached)
            {
                txbBatchNumber.Text = DateTime.Now.Day.ToString();
                txbFinancialYear.Text = DateTime.Now.Year.ToString();
                cboDatabase.Text = @"RentAndServ_Test";
                rdbActual.Checked = true;
            }
        }

        private void btnSearchPDFFile_Click(object sender, EventArgs e)
        {
            openFiledlg.ShowDialog();
            txbPDFFilePath.Text = openFiledlg.FileName;
        }

        private bool ValidateInput()
        {
            return txbPDFFilePath.Text != string.Empty
                   && cboDatabase.Text != string.Empty
                   && txbBatchNumber.Text != string.Empty
                   && txbFinancialYear.Text != string.Empty
                   && (rdbActual.Checked || rdbEstimation.Checked);
        }

        private bool ValidateFile()
        {
            if (string.IsNullOrEmpty(txbPDFFilePath.Text))
            {
                MessageBox.Show(@"There is no PDF file to open", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!File.Exists(txbPDFFilePath.Text))
            {
                MessageBox.Show($@"The file {txbPDFFilePath.Text} doesn't exist", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!Path.GetExtension(txbPDFFilePath.Text).Equals(".PDF", StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show($@"The file {txbPDFFilePath.Text} is not a PDF file", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            try
            {
                lblPDFStatus.Text = string.Empty;
                lblPDFStatus.Refresh();

                if (!ValidateInput())
                {
                    MessageBox.Show(@"One of the items has NOT been populated", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!ValidateFile())
                {
                    return;
                }

                Cursor.Current = Cursors.WaitCursor;
                btnProcess.Enabled = false;

                lblPDFStatus.Text = @"Loading PDF...";
                lblPDFStatus.Refresh();

                csvAdapter = new CSVAdapter();
                csvAdapter.Process(txtCsvFilePath.Text);

                var loadPdf = new PdfAdapter(txbPDFFilePath.Text,
                                       Convert.ToInt32(txtstartPage.Text), Convert.ToInt32(txtEndpage.Text));


                List<string> startLetter = new List<string>() { "Your payment reference" };//, "Your rent and service charge – important information", "Your rent – important information", "Privacy Notice"};
                List<string> endLetter = new List<string>() { "Your Services Charges Explained" };//, "Your monthly contributions towards the annual estimate will be £", "Your weekly charge will be £", "Your monthly charge will be £", "This will be taken off your estimate for" };

                StringBuilder lines = null;

                var alWorker = new ActualLetterWorker(lines)
                {
                    PDFStatus = lblPDFStatus,
                    ConnectionName = cboDatabase.Text,
                    FilePath = txbPDFFilePath.Text.Replace("'", "''")
                };

                validationReporter = new ValidationReporter(csvAdapter);

                int num = 1;

                while (loadPdf.PageNumber <= loadPdf.PageCount && loadPdf.PageNumber <= loadPdf.EndPageNumber)
                {
                    lines = loadPdf.ReadPDFLetter(startLetter, endLetter);

                    if (lines.Length == 0)
                    {
                        break;
                    }
                    else
                    {
                        lblPDFStatus.Text = $"PDF loaded.Validating Pdf with Csv File started at:{DateTime.Now.ToString("HH:mm dd/MM/yyyy ")}";
                        lblPDFStatus.Refresh();

                        alWorker.lines = lines;

                        alWorker.ProcessLetters(OnNotifyLetterProcessed, startLetter, endLetter);
                    }

                    alWorker.Clear();

                    //lblPDFStatus.Text = string.Format("PDF Count :{0}", num++);
                    lblPDFStatus.Refresh();

                    Application.DoEvents();
                }

               
                loadPdf.ClosePDF();


                lblPDFStatus.Text = $"Completed without Error at : {DateTime.Now.ToString("yyyyMMdd_HHmm")}";
                lblPDFStatus.Refresh();
            }
            catch (Exception ex)
            {
                lblPDFStatus.Text = @"An error has occurred. Rollback changes.";
                lblPDFStatus.Refresh();

                txbErrorDetail.Text = ex.ToString();
                txbErrorDetail.Refresh();
            }
            finally
            {
                btnProcess.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }

        private void WriteToFile(string filePath, string lines)
        {

            File.WriteAllText(filePath, lines.ToString());
        }
        public void OnNotifyLetterProcessed(int letterCount, RentAndServiceLetter rentServiceLetter)
        {

            if (!validationReporter.Process(rentServiceLetter, txbBatchNumber.Text))
            {
                //WriteToDebug("No Service Charge Records(SCH,SCD in csv file", rentServiceLetter.PropertyRef);
            }

            txtLetterProcessStatus.Text = txtLetterProcessStatus.Text + $"Processing Letter Number:{letterCount}\r\n";
            txtLetterProcessStatus.Text = txtLetterProcessStatus.Text + $"Property Reference:{rentServiceLetter?.PropertyRef}\r\n\r\n";


            txtLetterProcessStatus.SelectionStart = txtLetterProcessStatus.TextLength;
            txtLetterProcessStatus.ScrollToCaret();
            //var sch = csvAdapter.GetServiceChargesByPropertReference("191280");
            //var schList = csvAdapter.GetAllServiceCharges();
        }

        private void WriteToDebug(string lines, string propertyRef)
        {
            string fileName = $"F:\\ScanPDFLetters\\DebugFiles\\{propertyRef}_{DateTime.Now.ToString("yyyyMMdd_HHmm")}.txt";


            File.WriteAllText(fileName, lines.ToString());
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var csvAdapter = new CSVAdapter();

            csvAdapter.Process(txtCsvFilePath.Text);


        }

        private void btnSearchCsv_Click(object sender, EventArgs e)
        {
            csvFolderBrowserDialog.ShowDialog();
            txtCsvFilePath.Text = csvFolderBrowserDialog.FileName;
        }
    }
}
