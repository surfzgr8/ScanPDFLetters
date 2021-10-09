
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Windows.Forms;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ScanPDFLetters.Helper
{
    internal class PdfAdapter
    {
        private readonly string _filename;
        private readonly StringBuilder _lines;
        private readonly int? _endPageNumber;
        private int _pageCount;
        private int _totalPagesCount;

        private int _pageNumber;
        private PdfReader _reader = null;
        private StringBuilder _letter = null;
        private StringBuilder _currentLine = null;

        public PdfAdapter()
        {
            _lines = new StringBuilder();
        }

        public PdfAdapter(string filename)
            : this()
        {
            this._filename = filename;
        }

        public PdfAdapter(string filename, int? startPageNumber, int? endPageNumber)
        : this()
        {
            this._filename = filename;
            _pageNumber = startPageNumber ?? 0;
            _pageCount = endPageNumber ?? _reader.NumberOfPages;
            _endPageNumber = endPageNumber;
        }
        public StringBuilder GetPDFLines => _lines;

        public int PageCount => _pageCount;
        public int PageNumber => _pageNumber;
        public int? EndPageNumber => _endPageNumber;

        public StringBuilder ReadPdf()
        {
            using (var reader = new PdfReader(_filename))
            {
                // for (int pageNumber = 1; pageNumber <= reader.NumberOfPages; ++pageNumber)
                for (int pageNumber = 1; pageNumber <= _pageCount; ++pageNumber)
                {
                    Application.DoEvents();
                    var strategy = new LocationTextExtractionStrategy(new LocationTextExtractionStrategy.TextChunkLocationStrategyDefaultImp());
                    _lines.Append(PdfTextExtractor.GetTextFromPage(reader, pageNumber, strategy));
                }
                _pageCount = reader.NumberOfPages;
            }

            return _lines;
        }

        public StringBuilder ReadPDFLetter(List<string> startLetter, List<string> endLetter)
        {
            if (_reader == null)
            {
                _reader = new PdfReader(_filename);
                _letter = new StringBuilder();
                _currentLine = new StringBuilder();
                //_pageNumber = 1;
                //_pageNumber = 153661;
            }

            bool notFound = true;
            bool inLetter = false;
            //            var strategy = new LocationTextExtractionStrategy(new LocationTextExtractionStrategy.TextChunkLocationStrategyDefaultImp());

            _letter.Clear();
            _currentLine.Clear();

            //while (notFound && _pageNumber <= _reader.NumberOfPages)
            while (notFound && _pageNumber <= _pageCount && _pageNumber <=_endPageNumber && _pageNumber <=_reader.NumberOfPages)
            {
                Application.DoEvents();

                var strategy = new LocationTextExtractionStrategy(new LocationTextExtractionStrategy.TextChunkLocationStrategyDefaultImp());
                _currentLine.Append(PdfTextExtractor.GetTextFromPage(_reader, _pageNumber, strategy));

                if (!inLetter)
                {
                    if (FindMatch(_currentLine.ToString(), startLetter))
                    {
                        _letter.Append(_currentLine);
                        inLetter = true;
                    }
                }
                else
                {
                    _letter.Append(_currentLine);

                    if (FindMatch(_currentLine.ToString(), endLetter))
                        notFound = false;
                }

                _pageNumber++;
                _currentLine.Clear();
            }

            _pageCount = _reader.NumberOfPages;

            return _letter;
        }


        public void ClosePDF()
        {
            if (_reader != null)
                _reader.Close();
        }


        private bool FindMatch(string line, List<string> search)
        {
            bool result = false;

            foreach (string str in search)
            {
                if (line.IndexOf(str) != -1)
                {
                    result = true;
                }
            }

            return result;
        }

    }
}
