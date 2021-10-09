using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileHelpers;
using Csv = ScanPDFLetters.Model.Csv;
using Actual = ScanPDFLetters.Model.Actual;

namespace ScanPDFLetters.Helper
{
    public interface ICSVAdapter
    {
        void Process(string fileName);
        IEnumerable<Actual.ServiceChargeHeader> GetAllServiceCharges();
        IEnumerable<Actual.ServiceChargeHeader> GetServiceChargesByPropertReference(string propertyReference);
    }
    public class CSVAdapter : ICSVAdapter
    {
        private MultiRecordEngine _fileHelperEngine;
        private List<Actual.ServiceChargeHeader> _serviceChargeList;


        public CSVAdapter()
        {

            _serviceChargeList = new List<Actual.ServiceChargeHeader>();
        }

        public IEnumerable<Actual.ServiceChargeHeader> GetAllServiceCharges()
        {
            return _serviceChargeList
                 ?? throw new ArgumentNullException(nameof(_serviceChargeList), "ServiceChargeList has no elements");
        }

        public IEnumerable<Actual.ServiceChargeHeader> GetServiceChargesByPropertReference(string propertyReference)
        {
            return _serviceChargeList.Where<Actual.ServiceChargeHeader>(sch => sch.PropertyRef == propertyReference)
                ?? throw new ArgumentNullException(nameof(_serviceChargeList), $"Unable to find Servicecharge with Property Reference:{propertyReference}");
        }

        public void Process(string fileName)
        {
            int serviceChargeHeaderCount = 0;

            try
            {
                _fileHelperEngine = new MultiRecordEngine(typeof(Csv.PropertyHeader), typeof(Csv.ServiceChargeHeader), typeof(Csv.ServiceChargeDetail));

                _fileHelperEngine.RecordSelector = new RecordTypeSelector(RecordSelector);

                var results = _fileHelperEngine.ReadFile(fileName);

                foreach (var record in results)
                {
                    if (record is Csv.ServiceChargeHeader)
                    {
                        serviceChargeHeaderCount++;

                        var csvServiceChargeHeader = record as Csv.ServiceChargeHeader;

                        _serviceChargeList
                            .Add(new Actual.ServiceChargeHeader
                            {
                                SequenceNumber = csvServiceChargeHeader.SequenceNumber,
                                PropertyRef = csvServiceChargeHeader.PropertyRef,
                                RecordType = csvServiceChargeHeader.RecordType
                            });
                    }

                    if (record is Csv.ServiceChargeDetail)
                    {
                        var csvServiceChargeDetail = record as Csv.ServiceChargeDetail;

                        _serviceChargeList[serviceChargeHeaderCount - 1].ServiceChargeDetails
                            .Add(new Actual.ServiceChargeDetail
                            {
                                ActualCost = csvServiceChargeDetail.ActualCost,
                                EstimatedCost = csvServiceChargeDetail.EstimatedCost,
                                EstimatedSchemeCost = csvServiceChargeDetail.EstimatedSchemeCost,
                                PropertyRef = csvServiceChargeDetail.PropertyRef,
                                RecordType = csvServiceChargeDetail.RecordType,
                                SequenceNumber = csvServiceChargeDetail.SequenceNumber,
                                ServiceChargeDescription = csvServiceChargeDetail.ServiceChargeDescription,
                                ServiceChargeType = csvServiceChargeDetail.ServiceChargeType,
                                TotalCost = csvServiceChargeDetail.TotalCost
                            });                                               

                    }

                }


            }
            catch (Exception ex)
            {
                throw;
            }

        }


        private Type RecordSelector(MultiRecordEngine engine, string recordLine)
        {
            try
            {
                if (recordLine.Length == 0)
                    return null;

                if (recordLine.Substring(2, 3) == "PRO")
                    return typeof(Csv.PropertyHeader);
                else if (recordLine.Substring(2, 3) == "SCH")
                    return typeof(Csv.ServiceChargeHeader);
                else if (recordLine.Substring(2, 3) == "SCD")
                    return typeof(Csv.ServiceChargeDetail);
                else if (recordLine.Substring(2, 3) == "SFH")
                    return null;
                else if (recordLine.Substring(2, 3) == "SFD")
                    return null;
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw ;
            }
            finally
            {

            }

        }
    }
}
