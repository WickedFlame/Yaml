﻿using System;
using System.Collections.Generic;

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
            var root = new Token(null, 0);
            IToken token = root;

            while (line != null)
            {
                while (token.Parent != null && line.Indentation <= token.Indentation)
                {
                    if (token.Parent == null)
                    {
                        break;
                    }

                    token = token.Parent;
                }

                var function = ParserFunctionFactory.GetFunction(line);
                if(function != null)
                {
                    token = function(token, line);
                }

                line = _scanner.ScanNext();
            }

            return root;
        }

        static class ParserFunctionFactory
        {
            private static readonly Dictionary<string, Func<IToken, YamlLine, IToken>> _functions;

            static ParserFunctionFactory()
            {
                _functions = new Dictionary<string, Func<IToken, YamlLine, IToken>>();

                _functions.Add("ListPropertyValue", (token, line) =>
                {
                    // add new object to the tree
                    var list = new Token(null, line.Indentation, TokenType.ListItem);

                    // list item
                    token.Set(list);

                    var value = new ValueToken(line.Property, line.Value, line.Indentation + 2);
                    list.Set(value);

                    return list;
                });

                _functions.Add("ListProperty", (token, line) =>
                {
                    // add new list to the tree
                    var list = new Token(null, line.Indentation, TokenType.ListItem);
                    token.Set(list);

                    // object node to list
                    var child = new Token(line.Property, line.Indentation + 2);
                    list.Set(child);
                    return child;
                });

                _functions.Add("List", (token, line) =>
                {
                    var child = new ValueToken(line.Property, line.Value, line.Indentation + 2);
                    token.Set(child);

                    return token;
                });

                _functions.Add("PropertyValue", (token, line) =>
                {
                    var child = new ValueToken(line.Property, line.Value, line.Indentation);
                    token.Set(child);

                    return token;
                });

                _functions.Add("Property", (token, line) =>
                {
                    // add new object to the tree
                    var child = new Token(line.Property, line.Indentation);

                    token.Set(child);
                    return child;
                });
            }

            public static Func<IToken, YamlLine, IToken> GetFunction(YamlLine line)
            {
                if (line.IsListItem && !string.IsNullOrEmpty(line.Property) && !string.IsNullOrEmpty(line.Value))
                {
                    return _functions["ListPropertyValue"];
                }

                if (line.IsListItem && !string.IsNullOrEmpty(line.Property))
                {
                    return _functions["ListProperty"];
                }

                // simple list item eg: List<string>
                if (line.IsListItem)
                {
                    return _functions["List"];
                }

                // simple property with value
                if (!string.IsNullOrEmpty(line.Property) && !string.IsNullOrEmpty(line.Value))
                {
                    return _functions["PropertyValue"];
                }

                if (!string.IsNullOrEmpty(line.Property))
                {
                    return _functions["Property"];
                }

                return null;
            }
        }
    }
}
