using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_New1
{
    public class Clock : BindableBase
    {

        string date;
        string time;

        public Clock()
        {
            Update();
        }

        public void Update()
        {
            Date = DateTime.Now.ToLongDateString();
            Time = DateTime.Now.ToLongTimeString();
        }

        public string Date
        {
            get { return date; }
            private set
            {
                SetProperty(ref date, value);
            }
        }

        public string Time
        {
            get { return time; }
            private set
            {
                SetProperty(ref time, value);
            }
        }
    }
}
