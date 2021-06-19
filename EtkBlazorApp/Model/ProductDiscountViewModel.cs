﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EtkBlazorApp
{
    public class ProductDiscountViewModel : ProductViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool preventUpdate = true;

        private decimal newPriceInRub;
        public decimal NewPriceInRub
        {
            get => newPriceInRub;
            set
            {
                if (newPriceInRub != value && Price != 0)
                {
                    newPriceInRub = value;                    
                    if (preventUpdate == false)
                    {
                        discountPercent = (int)((1 - (NewPriceInRub / Price)) * 100);
                        RaisePropertyChanged();
                    }
                }
            }
        }

        private decimal newPriceInCurrency;
        public decimal NewPriceInCurrency
        {
            get => newPriceInCurrency;
            set
            {
                if (newPriceInCurrency != value && BasePrice != 0)
                {
                    newPriceInCurrency = value;                 
                    if (preventUpdate == false)
                    {
                        discountPercent = (int)((1 - (newPriceInCurrency / BasePrice)) * 100);
                        RaisePropertyChanged();
                    }
                }
            }
        }

        public DateTime DiscountStartDate { get; set; } = DateTime.Now.Date;
        public DateTime DiscountEndDate { get; set; } = DateTime.Now.Date;
        public bool IsExpired => DaysLeft == 0;

        public int DaysLeft
        {
            get
            {
                if(DiscountEndDate > DateTime.Now.Date)
                {
                    return (int)Math.Floor((DiscountEndDate - DateTime.Now.Date).TotalDays);
                }

                return 0;
            }
        }

        int discountPercent;
        

        public new int DiscountPercent
        {
            get => discountPercent;
            set
            {
                if (discountPercent != value)
                {
                    discountPercent = value;

                    preventUpdate = true;
                    NewPriceInRub = (int)((base.Price / (100 + discountPercent)) * 100);
                    NewPriceInCurrency = (int)((base.BasePrice / (100 + discountPercent)) * 100);
                    preventUpdate = false;

                    RaisePropertyChanged();
                }
            }
        }

        public double PriceInRubDiscountPercent
        {
            get
            {
                if (Price == decimal.Zero)
                {
                    return 0;
                }
                return (double)(1 - NewPriceInRub / Price);
            }
        }
        public double PriceInCurrencyDiscountPercent
        {
            get
            {
                if (BasePrice == decimal.Zero)
                {
                    return 0;
                }
                return (double)(1 - NewPriceInCurrency / BasePrice);
            }
        }

        public bool IsValidDiscount
        {
            get
            {
                return Id != 0 && (newPriceInCurrency != 0 || newPriceInRub != 0) && DiscountEndDate >= DateTime.Now.Date;
            }
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
