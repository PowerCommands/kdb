using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PainKiller.SearchLib.DomainObjects;
using PainKiller.SearchLib.Indexing;

namespace PainKiller.SearchLib.Managers;
public class BM25SearchEngine
{
    private readonly List<Document> _documents = [];
    private readonly Dictionary<string, double> _idf = new();
    private readonly Dictionary<string, Dictionary<string, double>> _tf = new();
    private double _avgDocLength;
    public void LoadIndex(List<Document> savedIndex)
    {
        _documents.Clear();
        if (savedIndex.Count == 0)
        {
            Console.WriteLine("No index to load.");
            return;
        }
        _documents.AddRange(savedIndex);

        Console.WriteLine($"Index loaded: {_documents.Count} dokument.");
        CalculateBM25Parameters();
    }
    public List<SearchResult> Search(string query, int topN, List<IIndexManager> indexManagers)
    {
        var queryTerms = Tokenize(query);
        var scores = new Dictionary<string, double>();
        double k1 = 1.5, b = 0.75;

        foreach (var doc in _documents)
        {
            var docId = doc.DocId;
            var words = doc.Tokens;
            double docLength = words.Length;
            double score = 0;

            foreach (var term in queryTerms)
            {
                var closestMatch = GetClosestMatch(term, words);
                if (closestMatch == null) continue;

                var termFreq = _tf[docId].ContainsKey(closestMatch) ? _tf[docId][closestMatch] : 0;
                var idfScore = _idf.GetValueOrDefault(closestMatch, 0);
                var numerator = termFreq * (k1 + 1);
                var denominator = termFreq + k1 * (1 - b + b * (docLength / _avgDocLength));
                score += idfScore * (numerator / denominator);
            }
            if (score > 0) scores[docId] = score;
        }

        var topResults = scores.OrderByDescending(s => s.Value).Take(topN).Select(s =>
        {
            var matchedDocument = indexManagers.SelectMany(m => m.GetDocuments()).FirstOrDefault(d => d.DocId == s.Key);
            var matchingIndexManager = matchedDocument != null ? indexManagers.FirstOrDefault(m => m.GetDocumentType() == matchedDocument.Type) : null;

            return new SearchResult { DocId = s.Key, Score = s.Value, PageNumber = matchedDocument?.PageNumber, Content = matchingIndexManager != null ? matchingIndexManager.GetSurroundingText(s.Key) : "[Fel: Ingen indexhanterare hittad]" };
        }).ToList();

        return topResults;
    }
    private void CalculateBM25Parameters()
    {
        double totalLength = 0;
        var docCount = _documents.Count;

        foreach (var doc in _documents)
        {
            var docId = doc.DocId;
            var words = doc.Tokens;

            if (!_tf.ContainsKey(docId))
                _tf[docId] = new Dictionary<string, double>();

            foreach (var word in words)
            {
                if (!_tf[docId].ContainsKey(word))
                    _tf[docId][word] = 0;
                _tf[docId][word]++;
            }

            totalLength += words.Length;
        }

        _avgDocLength = totalLength / docCount;

        foreach (var term in _tf.SelectMany(d => d.Value.Keys).Distinct())
        {
            int docFreq = _tf.Count(d => d.Value.ContainsKey(term));
            _idf[term] = Math.Log((docCount - docFreq + 0.5) / (docFreq + 0.5) + 1);
        }
    }
    private string[] Tokenize(string text) => Regex.Split(text.ToLower(), @"\W+").Where(w => w.Length > 1).ToArray();
    private string? GetClosestMatch(string word, string[] words)
    {
        foreach (var w in words)
        {
            if (w.StartsWith(word) || w.Contains(word))
                return w;
        }
        return words.FirstOrDefault(w => LevenshteinDistance(word, w) <= 2);
    }
    private int LevenshteinDistance(string s, string t)
    {
        int[,] d = new int[s.Length + 1, t.Length + 1];

        for (int i = 0; i <= s.Length; i++) d[i, 0] = i;
        for (int j = 0; j <= t.Length; j++) d[0, j] = j;

        for (int i = 1; i <= s.Length; i++)
        {
            for (int j = 1; j <= t.Length; j++)
            {
                int cost = s[i - 1] == t[j - 1] ? 0 : 1;
                d[i, j] = Math.Min(Math.Min(
                    d[i - 1, j] + 1,      // deletion
                    d[i, j - 1] + 1),     // insertion
                    d[i - 1, j - 1] + cost); // substitution
            }
        }

        return d[s.Length, t.Length];
    }
}
