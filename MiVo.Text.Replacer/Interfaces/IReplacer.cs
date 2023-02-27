using System;

namespace MiVo.Text.Replacer.Interfaces
{
    public interface IReplacer
    {
        string GetText<T>(T obj);
        string GetText(object? obj = null, Type? t = null);
        void AddString(string pattern, string replacement);
        void AddStringReplacement(string key, string replacement);
        void AddImageReplacement(string key, string replacement);
        void AddRemoveRelation(string key, bool val);
        void AddReplacement(string key, IReplacement replacement);
    }
}