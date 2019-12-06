﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WickedFlame.Yaml
{
    public class Parser
    {
        private readonly IScanner _scanner;

        public Parser(IScanner scanner)
        {
            _scanner = scanner;
        }

        public IToken Parse()
        {
            var line = _scanner.ScanNext();
            var root = new Token(0);
            IToken token = root;

            while (line != null)
            {
                while (line.Indentation <= token.Indentation)
                {
                    if (token.Parent == null)
                    {
                        break;
                    }

                    token = token.Parent;
                }
                
                if (line.IsListItem && !string.IsNullOrEmpty(line.Property) && !string.IsNullOrEmpty(line.Value))
                {
                    // add new object to the tree
                    var child = new Token(line.Indentation, TokenType.List);

                    // list item
                    token.Set(null, child);
                    token = child;

                    var value = new ValueToken(line.Value, line.Indentation);
                    token.Set(line.Property, value);

                    line = _scanner.ScanNext();
                    continue;
                }



                if (line.IsListItem && !string.IsNullOrEmpty(line.Property))
                {
                    // add new list to the tree
                    var child = new Token(line.Indentation, TokenType.List);
                    token.Set(null, child);
                    token = child;


                    // object node to list
                    child = new Token(line.Indentation);
                    token.Set(line.Property, child);
                    token = child;



                    line = _scanner.ScanNext();
                    continue;
                }


                // simple list item eg: List<string>
                if (line.IsListItem)
                {
                    var child = new ValueToken(line.Value, line.Indentation);
                    token.Set(line.Property, child);

                    line = _scanner.ScanNext();
                    continue;
                }


                // simple property with value
                if (!string.IsNullOrEmpty(line.Property) && !string.IsNullOrEmpty(line.Value))
                {
                    var child = new ValueToken(line.Value, line.Indentation);
                    token.Set(line.Property, child);

                    line = _scanner.ScanNext();
                    continue;
                }

                if (!string.IsNullOrEmpty(line.Property))
                {
                    // add new object to the tree
                    var child = new Token(line.Indentation);

                    token.Set(line.Property, child);
                    token = child;

                    line = _scanner.ScanNext();
                    continue;
                }

                line = _scanner.ScanNext();
            }

            return root;
        }
    }

    public interface IScanner
    {
        YamlLine ScanNext();
    }

    public class Scanner : IScanner
    {
        private readonly string[] _input;
        private int _index;

        public Scanner(string[] input)
        {
            _input = input;
            _index = -1;
        }

        public YamlLine ScanNext()
        {
            _index = _index + 1;
            if (_index >= _input.Length)
            {
                return null;
            }

            var line = new YamlLine(_input[_index]);
            return line;
        }
    }

    public class TokenValue
    {
        public TokenValue(string key, IToken value)
        {
            Key = key;
            Token = value;
        }

        public string Key { get; }

        public IToken Token { get; }

        public override string ToString()
        {
            return $"TokenValue - Key:{Key} Token:{Token}";
        }
    }

    public interface IToken
    {
        TokenType TokenType { get; }

        IToken this[string key] { get; }

        IToken this[int index] { get; }

        int Indentation { get; }

        IToken Parent { get; set; }

        void Set(string key, IToken value);
    }

    public class Token : IToken
    {
        //private readonly Dictionary<string, IToken> _children = new Dictionary<string, IToken>();
        private readonly List<TokenValue> _children = new List<TokenValue>();

        public Token(int indentaiton, TokenType tokenType)
        {
            TokenType = tokenType;
            Indentation = indentaiton;
        }

        public Token(int indentaiton)
        {
            TokenType = TokenType.Object;
            Indentation = indentaiton;
        }

        public TokenType TokenType { get; }

        public IToken this[string key]
        {
            get
            {
                return _children.FirstOrDefault(t => t.Key == key)?.Token;
            }
        }

        public IToken this[int index]
        {
            get
            {
                var value = _children.ElementAt(index);
                return value?.Token;
            }
        }

        public int Indentation { get; set; }

        public IToken Parent { get; set; }

        public void Set(string key, IToken token)
        {
            if(!string.IsNullOrEmpty(key))
            {
                var item = _children.FirstOrDefault(v => v.Key == key);
                if (item != null)
                {
                    _children.Remove(item);
                }
            }

            token.Parent = this;

            _children.Add(new TokenValue(key, token));
        }

        public override string ToString()
        {
            return $"TokenType: {TokenType}";
        }
    }

    public class ValueToken : IToken
    {
        private object _value;

        public ValueToken(object value, int indentation)
        {
            TokenType = TokenType.Value;
            _value = value;
            Indentation = indentation;
        }

        public TokenType TokenType { get; }

        public IToken this[string key]
        {
            get
            {
                throw new InvalidOperationException("A ValueToken cannot be used with a Indexer");
            }
        }

        public IToken this[int index]
        {
            get
            {
                throw new InvalidOperationException("A ValueToken cannot be used with a Indexer");
            }
        }

        public int Indentation { get; set; }

        public IToken Parent { get; set; }

        public object Value => _value;

        public void Set(string key, IToken value)
        {
        }

        public override string ToString()
        {
            return $"ValueToken: {_value}";
        }
    }

    public enum TokenType
    {
        Value,
        Object,
        List
    }
}
