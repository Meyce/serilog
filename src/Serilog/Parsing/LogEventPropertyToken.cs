﻿// Copyright 2013 Nicholas Blumhardt
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Serilog.Events;

namespace Serilog.Parsing
{
    /// <summary>
    /// A message template token representing a log event property.
    /// </summary>
    public class LogEventPropertyToken : MessageTemplateToken
    {
        private readonly string _propertyName;
        private readonly string _format;
        private readonly Destructuring _destructuring;
        private readonly string _rawText;

        /// <summary>
        /// Construct a <see cref="LogEventPropertyToken"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="rawText">The token as it appears in the message template.</param>
        /// <param name="format">The format applied to the property, if any.</param>
        /// <param name="destructuring">The destructuring strategy applied to the property, if any.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public LogEventPropertyToken(string propertyName, string rawText, string format = null, Destructuring destructuring = Destructuring.Default)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            if (rawText == null) throw new ArgumentNullException("rawText");
            _propertyName = propertyName;
            _format = format;
            _destructuring = destructuring;
            _rawText = rawText;
        }

        /// <summary>
        /// Render the token to the output.
        /// </summary>
        /// <param name="properties">Properties that may be represented by the token.</param>
        /// <param name="output">Output for the rendered string.</param>
        public override void Render(IReadOnlyDictionary<string, LogEventProperty> properties, TextWriter output)
        {
            if (properties == null) throw new ArgumentNullException("properties");
            if (output == null) throw new ArgumentNullException("output");
            LogEventProperty property;
            if (properties.TryGetValue(_propertyName, out property))
                property.Value.Render(output, _format);
            else
                output.Write(_rawText);
        }

        /// <summary>
        /// The property name.
        /// </summary>
        public string PropertyName { get { return _propertyName; } }

        /// <summary>
        /// Destructuring strategy applied to the property.
        /// </summary>
        public Destructuring Destructuring { get { return _destructuring; } }

        /// <summary>
        /// Format applied to the property.
        /// </summary>
        public string Format { get { return _format; } }
        
        /// <summary>
        /// True if the property name is a positional index; otherwise, false.
        /// </summary>
        public bool IsPositional
        {
            get { return _propertyName.All(char.IsNumber); }
        }

        /// <summary>
        /// Try to get the integer value represented by the property name.
        /// </summary>
        /// <param name="position">The integer value, if present.</param>
        /// <returns>True if the property is positional, otherwise false.</returns>
        public bool TryGetPositionalValue(out int position)
        {
            return
                int.TryParse(_propertyName, NumberStyles.None, CultureInfo.InvariantCulture, out position) &&
                position >= 0;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            var pt = obj as LogEventPropertyToken;
            return pt != null &&
                pt._destructuring == _destructuring &&
                pt._format == _format &&
                pt._propertyName == _propertyName &&
                pt._rawText == _rawText;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return _propertyName.GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return _rawText;
        }
    }
}