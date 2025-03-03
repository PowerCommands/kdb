using System;
using PdfSharpTextExtractor;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PainKiller.SearchLib.Managers;

public class PdfToTextManager
{
    public static void ConvertPdfToText(string directory)
    {
        var pdfFiles = Directory.GetFiles(directory, "*.pdf");
        foreach (var file in pdfFiles)
        {
            var rawText = PdfToText(file);
            rawText = Regex.Replace(rawText, @"<[^>]+>", ""); 
            var textLines = rawText.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);


            var textFileName = file.Replace(".pdf", ".txt");
            File.WriteAllText(textFileName, rawText);
            Console.WriteLine($"{textFileName} created.");
        }
    }
    public static string PdfToText(string file)
    {
        StringBuilder res = new StringBuilder();
        using (PdfDocument doc = PdfReader.Open(file, PdfDocumentOpenMode.ReadOnly))
        {
            Extractor extractor = new Extractor(doc);
            var pageCount = 1;
            foreach (PdfPage page in doc.Pages)
            {
                res.AppendLine($"PageCounter {pageCount}");
                res.AppendLine();
                extractor.ExtractText(page, res);
                res.AppendLine();
                pageCount++;
            }
        }
        return res.ToString();
    }
}