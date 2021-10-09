using System.ComponentModel;
using System.Windows.Forms;

namespace ScanPDFLetters
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblPDFFilePath = new System.Windows.Forms.Label();
            this.openFiledlg = new System.Windows.Forms.OpenFileDialog();
            this.txbPDFFilePath = new System.Windows.Forms.TextBox();
            this.btnSearchPDFFile = new System.Windows.Forms.Button();
            this.btnProcess = new System.Windows.Forms.Button();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.cboDatabase = new System.Windows.Forms.ComboBox();
            this.lblBatchNo = new System.Windows.Forms.Label();
            this.txbBatchNumber = new System.Windows.Forms.TextBox();
            this.lblFinancialYear = new System.Windows.Forms.Label();
            this.txbFinancialYear = new System.Windows.Forms.TextBox();
            this.rdbEstimation = new System.Windows.Forms.RadioButton();
            this.rdbActual = new System.Windows.Forms.RadioButton();
            this.txbErrorDetail = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSearchCsv = new System.Windows.Forms.Button();
            this.txtCsvFilePath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.csvFolderBrowserDialog = new System.Windows.Forms.OpenFileDialog();
            this.txtLetterProcessStatus = new System.Windows.Forms.TextBox();
            this.lblPDFStatus = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtstartPage = new System.Windows.Forms.TextBox();
            this.txtEndpage = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblPDFFilePath
            // 
            this.lblPDFFilePath.AutoSize = true;
            this.lblPDFFilePath.Location = new System.Drawing.Point(13, 17);
            this.lblPDFFilePath.Name = "lblPDFFilePath";
            this.lblPDFFilePath.Size = new System.Drawing.Size(117, 13);
            this.lblPDFFilePath.TabIndex = 0;
            this.lblPDFFilePath.Text = "Open PDF Letters File: ";
            // 
            // openFiledlg
            // 
            this.openFiledlg.DefaultExt = "pdf";
            this.openFiledlg.Filter = "PDF files|*.pdf";
            this.openFiledlg.Title = "Open PDF file";
            // 
            // txbPDFFilePath
            // 
            this.txbPDFFilePath.Location = new System.Drawing.Point(136, 13);
            this.txbPDFFilePath.Name = "txbPDFFilePath";
            this.txbPDFFilePath.Size = new System.Drawing.Size(403, 20);
            this.txbPDFFilePath.TabIndex = 1;
            // 
            // btnSearchPDFFile
            // 
            this.btnSearchPDFFile.Location = new System.Drawing.Point(545, 12);
            this.btnSearchPDFFile.Name = "btnSearchPDFFile";
            this.btnSearchPDFFile.Size = new System.Drawing.Size(30, 23);
            this.btnSearchPDFFile.TabIndex = 2;
            this.btnSearchPDFFile.Text = "...";
            this.btnSearchPDFFile.UseVisualStyleBackColor = true;
            this.btnSearchPDFFile.Click += new System.EventHandler(this.btnSearchPDFFile_Click);
            // 
            // btnProcess
            // 
            this.btnProcess.Location = new System.Drawing.Point(12, 182);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(566, 23);
            this.btnProcess.TabIndex = 3;
            this.btnProcess.Text = "Process";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(9, 96);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(53, 13);
            this.lblDatabase.TabIndex = 4;
            this.lblDatabase.Text = "Database";
            this.lblDatabase.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cboDatabase
            // 
            this.cboDatabase.FormattingEnabled = true;
            this.cboDatabase.Items.AddRange(new object[] {
            "RentAndServ_UAT",
            "RentAndServ_Test",
            "RentandServ_Live"});
            this.cboDatabase.Location = new System.Drawing.Point(84, 93);
            this.cboDatabase.Name = "cboDatabase";
            this.cboDatabase.Size = new System.Drawing.Size(121, 21);
            this.cboDatabase.TabIndex = 5;
            // 
            // lblBatchNo
            // 
            this.lblBatchNo.AutoSize = true;
            this.lblBatchNo.Location = new System.Drawing.Point(243, 99);
            this.lblBatchNo.Name = "lblBatchNo";
            this.lblBatchNo.Size = new System.Drawing.Size(75, 13);
            this.lblBatchNo.TabIndex = 6;
            this.lblBatchNo.Text = "Batch Number";
            // 
            // txbBatchNumber
            // 
            this.txbBatchNumber.Location = new System.Drawing.Point(324, 95);
            this.txbBatchNumber.Name = "txbBatchNumber";
            this.txbBatchNumber.Size = new System.Drawing.Size(82, 20);
            this.txbBatchNumber.TabIndex = 7;
            // 
            // lblFinancialYear
            // 
            this.lblFinancialYear.AutoSize = true;
            this.lblFinancialYear.Location = new System.Drawing.Point(427, 99);
            this.lblFinancialYear.Name = "lblFinancialYear";
            this.lblFinancialYear.Size = new System.Drawing.Size(74, 13);
            this.lblFinancialYear.TabIndex = 8;
            this.lblFinancialYear.Text = "Financial Year";
            // 
            // txbFinancialYear
            // 
            this.txbFinancialYear.Location = new System.Drawing.Point(507, 95);
            this.txbFinancialYear.Name = "txbFinancialYear";
            this.txbFinancialYear.Size = new System.Drawing.Size(71, 20);
            this.txbFinancialYear.TabIndex = 9;
            // 
            // rdbEstimation
            // 
            this.rdbEstimation.AutoSize = true;
            this.rdbEstimation.Location = new System.Drawing.Point(12, 121);
            this.rdbEstimation.Name = "rdbEstimation";
            this.rdbEstimation.Size = new System.Drawing.Size(73, 17);
            this.rdbEstimation.TabIndex = 11;
            this.rdbEstimation.TabStop = true;
            this.rdbEstimation.Text = "Estimation";
            this.rdbEstimation.UseVisualStyleBackColor = true;
            // 
            // rdbActual
            // 
            this.rdbActual.AutoSize = true;
            this.rdbActual.Location = new System.Drawing.Point(120, 121);
            this.rdbActual.Name = "rdbActual";
            this.rdbActual.Size = new System.Drawing.Size(55, 17);
            this.rdbActual.TabIndex = 12;
            this.rdbActual.TabStop = true;
            this.rdbActual.Text = "Actual";
            this.rdbActual.UseVisualStyleBackColor = true;
            // 
            // txbErrorDetail
            // 
            this.txbErrorDetail.Location = new System.Drawing.Point(12, 730);
            this.txbErrorDetail.MaxLength = 1000000;
            this.txbErrorDetail.Multiline = true;
            this.txbErrorDetail.Name = "txbErrorDetail";
            this.txbErrorDetail.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txbErrorDetail.Size = new System.Drawing.Size(556, 77);
            this.txbErrorDetail.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 714);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Error Details";
            // 
            // btnSearchCsv
            // 
            this.btnSearchCsv.Location = new System.Drawing.Point(545, 45);
            this.btnSearchCsv.Name = "btnSearchCsv";
            this.btnSearchCsv.Size = new System.Drawing.Size(30, 23);
            this.btnSearchCsv.TabIndex = 18;
            this.btnSearchCsv.Text = "...";
            this.btnSearchCsv.UseVisualStyleBackColor = true;
            this.btnSearchCsv.Click += new System.EventHandler(this.btnSearchCsv_Click);
            // 
            // txtCsvFilePath
            // 
            this.txtCsvFilePath.Location = new System.Drawing.Point(136, 48);
            this.txtCsvFilePath.Name = "txtCsvFilePath";
            this.txtCsvFilePath.Size = new System.Drawing.Size(403, 20);
            this.txtCsvFilePath.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Open Csv File: ";
            // 
            // csvFolderBrowserDialog
            // 
            this.csvFolderBrowserDialog.Filter = "Csv File | *.csv";
            // 
            // txtLetterProcessStatus
            // 
            this.txtLetterProcessStatus.Location = new System.Drawing.Point(12, 253);
            this.txtLetterProcessStatus.Multiline = true;
            this.txtLetterProcessStatus.Name = "txtLetterProcessStatus";
            this.txtLetterProcessStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLetterProcessStatus.Size = new System.Drawing.Size(566, 458);
            this.txtLetterProcessStatus.TabIndex = 19;
            // 
            // lblPDFStatus
            // 
            this.lblPDFStatus.AutoSize = true;
            this.lblPDFStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPDFStatus.ForeColor = System.Drawing.Color.Green;
            this.lblPDFStatus.Location = new System.Drawing.Point(16, 820);
            this.lblPDFStatus.Name = "lblPDFStatus";
            this.lblPDFStatus.Size = new System.Drawing.Size(0, 13);
            this.lblPDFStatus.TabIndex = 20;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 228);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Progress Status";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 157);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Start Page";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtstartPage
            // 
            this.txtstartPage.Location = new System.Drawing.Point(84, 154);
            this.txtstartPage.Name = "txtstartPage";
            this.txtstartPage.Size = new System.Drawing.Size(204, 20);
            this.txtstartPage.TabIndex = 23;
            this.txtstartPage.Text = "1";
            // 
            // txtEndpage
            // 
            this.txtEndpage.Location = new System.Drawing.Point(386, 154);
            this.txtEndpage.Name = "txtEndpage";
            this.txtEndpage.Size = new System.Drawing.Size(189, 20);
            this.txtEndpage.TabIndex = 25;
            this.txtEndpage.Text = "5000";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(310, 157);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "End Page";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 846);
            this.Controls.Add(this.txtEndpage);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtstartPage);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblPDFStatus);
            this.Controls.Add(this.txtLetterProcessStatus);
            this.Controls.Add(this.btnSearchCsv);
            this.Controls.Add(this.txtCsvFilePath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txbErrorDetail);
            this.Controls.Add(this.rdbActual);
            this.Controls.Add(this.rdbEstimation);
            this.Controls.Add(this.txbFinancialYear);
            this.Controls.Add(this.lblFinancialYear);
            this.Controls.Add(this.txbBatchNumber);
            this.Controls.Add(this.lblBatchNo);
            this.Controls.Add(this.cboDatabase);
            this.Controls.Add(this.lblDatabase);
            this.Controls.Add(this.btnProcess);
            this.Controls.Add(this.btnSearchPDFFile);
            this.Controls.Add(this.txbPDFFilePath);
            this.Controls.Add(this.lblPDFFilePath);
            this.Name = "frmMain";
            this.Text = "Read PDF and write to SQL";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label lblPDFFilePath;
        private OpenFileDialog openFiledlg;
        private TextBox txbPDFFilePath;
        private Button btnSearchPDFFile;
        private Button btnProcess;
        private Label lblDatabase;
        private ComboBox cboDatabase;
        private Label lblBatchNo;
        private TextBox txbBatchNumber;
        private Label lblFinancialYear;
        private TextBox txbFinancialYear;
        private RadioButton rdbEstimation;
        private RadioButton rdbActual;
        private TextBox txbErrorDetail;
        private Label label1;
        private Button btnSearchCsv;
        private TextBox txtCsvFilePath;
        private Label label2;
        private OpenFileDialog csvFolderBrowserDialog;
        private TextBox txtLetterProcessStatus;
        private Label lblPDFStatus;
        private Label label3;
        private Label label4;
        private TextBox txtstartPage;
        private TextBox txtEndpage;
        private Label label5;
    }
}

