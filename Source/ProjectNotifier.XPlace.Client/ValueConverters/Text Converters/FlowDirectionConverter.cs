﻿namespace ProjectNotifier.XPlace.Client
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Windows;


    /// <summary>
    /// Sets the flow direction of the text depending on what type of language was enter in the first letter
    /// </summary>
    public class FlowDirectionConverter : BaseValueConverter<FlowDirectionConverter>
    {

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Gets the text as a string
            string text = (string)value;

            // Checks if the string is empty null or contains only white spaces
            if (string.IsNullOrWhiteSpace(text))
                // Returns the default value for the control which is right to left because Israel
                return FlowDirection.RightToLeft;


            // Checks if the first characters matches the pattern 
            bool regexMatch = Regex.Match(text[0].ToString(), @"[\u0590-\u05FF]").Success;


            // If the first character is hebrew (matches the regex pattern)
            if (regexMatch == true)
            {
                // Return a flow direction of right to left
                return FlowDirection.RightToLeft;
            }
            else
            {
                return FlowDirection.LeftToRight;
            };

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
