using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTAddin;
using Model.Form;

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

        private Form form;
        public Form Form => form ?? (form = new Form { DataContext = this.TColumnGroupVM });

        private TColumnGroupVM tColumnGroupVM;
        public TColumnGroupVM TColumnGroupVM => this.tColumnGroupVM ?? (this.tColumnGroupVM = TColumnGroupVMUtil.GetVM(ModelData.TColumnGroup));

    }
}
