using System;

namespace JumpsellerSync.BusinessLogic.Core.Dtos
{
    public class ConvertCurrencyDetailsDto
    {
        private double _value;

        public string From { get; set; }

        public string To { get; set; }

        public double Value
        {
            get => _value;
            set => _value = Math.Round(value, 2);
        }
    }
}
