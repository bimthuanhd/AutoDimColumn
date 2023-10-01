using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTAddin;

namespace SingleData
{
    public class FormData
    {
        private static FormData instance;
        public static FormData Instance
        {
            get=> instance ?? (instance = new FormData());
            set => instance = value;
        }

        //model
        public  Data ModelData => Data.Instance;
         


    }
}
