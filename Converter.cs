using System;
using System.Collections.Generic;
using System.Text;

public static class Converter
{
    private static readonly Dictionary<char, char> EnToHeMap = new Dictionary<char, char>
    {
        {'q','/'}, {'w','\''}, {'e','\u05E7'}, {'r','\u05E8'}, {'t','\u05D0'}, 
        {'y','\u05D8'}, {'u','\u05D5'}, {'i','\u05DF'}, {'o','\u05DD'}, {'p','\u05E4'},
        {'a','\u05E9'}, {'s','\u05D3'}, {'d','\u05D2'}, {'f','\u05DB'}, {'g','\u05E2'}, 
        {'h','\u05D9'}, {'j','\u05D7'}, {'k','\u05DC'}, {'l','\u05DA'}, {';','\u05E3'}, 
        {'z','\u05D6'}, {'x','\u05E1'}, {'c','\u05D1'}, {'v','\u05D4'}, {'b','\u05E0'}, 
        {'n','\u05DE'}, {'m','\u05E6'}, {',','\u05EA'}, {'.','\u05E5'}, {'/','.'}, 
        
        {'Q','\u05E7'}, {'W','\u05E8'}, {'E','\u05D0'}, {'R','\u05D8'}, {'T','\u05D5'}, 
        {'Y','\u05DF'}, {'U','\u05DD'}, {'I','\u05E4'}, {'O','['}, {'P',']'},
        {'A','\u05E9'}, {'S','\u05D3'}, {'D','\u05D2'}, {'F','\u05DB'}, {'G','\u05E2'}, 
        {'H','\u05D9'}, {'J','\u05D7'}, {'K','\u05DC'}, {'L','\u05DA'}, {':','\u05E3'}, 
        {'Z','\u05D6'}, {'X','\u05E1'}, {'C','\u05D1'}, {'V','\u05D4'}, {'B','\u05E0'}, 
        {'N','\u05DE'}, {'M','\u05E6'}, {'<','\u05EA'}, {'>','\u05E5'},
        
        {'[',']'}, {']','['}, {'\'',','}, {'{','}'}, {'}','{'}, {'"',','}, {'?','.'}
    };

    private static readonly Dictionary<char, char> HeToEnMap = new Dictionary<char, char>
    {
        {'\u05E7','e'}, {'\u05E8','r'}, {'\u05D0','t'}, {'\u05D8','y'}, {'\u05D5','u'}, 
        {'\u05DF','i'}, {'\u05DD','o'}, {'\u05E4','p'}, {'\u05E9','a'}, {'\u05D3','s'}, 
        {'\u05D2','d'}, {'\u05DB','f'}, {'\u05E2','g'}, {'\u05D9','h'}, {'\u05D7','j'}, 
        {'\u05DC','k'}, {'\u05DA','l'}, {'\u05E3',';'}, {'\u05D6','z'}, {'\u05E1','x'}, 
        {'\u05D1','c'}, {'\u05D4','v'}, {'\u05E0','b'}, {'\u05DE','n'}, {'\u05E6','m'}, 
        {'\u05EA',','}, {'\u05E5','.'}, 
        
        {'/','q'}, {'\'','w'}, {',','\''}, {'.','/'}, {']','['}, {'[',']'}, {'}','{'}, {'{','}'}
    };

    public static string ConvertLayout(string input, string direction)
    {
        var map = direction == "en_to_he" ? EnToHeMap : HeToEnMap;
        var output = new StringBuilder();
        foreach (char c in input)
            output.Append(map.ContainsKey(c) ? map[c] : c);
        return output.ToString();
    }

    public static string SwapCase(string input)
    {
        var result = new StringBuilder(input.Length);
        foreach (char c in input)
        {
            if (char.IsUpper(c)) result.Append(char.ToLowerInvariant(c));
            else if (char.IsLower(c)) result.Append(char.ToUpperInvariant(c));
            else result.Append(c);
        }
        return result.ToString();
    }

    public static string DetectTextDirection(string text)
    {
        if (string.IsNullOrEmpty(text)) return "en_to_he";
        int enCount = 0, heCount = 0;
        foreach (char c in text)
        {
            if (EnToHeMap.ContainsKey(c)) heCount++;
            else if (HeToEnMap.ContainsKey(c)) enCount++;
        }
        return heCount > enCount ? "en_to_he" : "he_to_en";
    }
}