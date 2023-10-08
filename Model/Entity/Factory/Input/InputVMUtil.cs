using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleData;

namespace HTAddin
{
    public class InputVMUtil
    {
        public static InputVM GetVM(Input q, TWallColumnGroupVM tColumnGroupVM)
        {
            return new InputVM
            {
                ModelItem = q,
                TWallColumnGroupVM = tColumnGroupVM
            };
        }

    }
}
